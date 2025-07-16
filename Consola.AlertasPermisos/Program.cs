using Business.Implementacion;
using Business.Interfaces;
using Data.DBContext;
using Data.Implementacion;
using Data.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Permisos.WebApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Permisos.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddDbContext<PermisosContext>(options =>
            {
                options.UseSqlServer("Server=localhost;Initial Catalog=db_Permisos;User ID=sa;Password=Laurean0;TrustServerCertificate=True");
            });
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IAlertaService, AlertaService>();
            services.AddScoped<IPermisoService, PermisoService>();
            services.AddScoped<IDestinatarioService, DestinatarioService>();
            services.AddScoped<INegocioService, NegocioService>();
            services.AddScoped<IPlantillaService, PlantillaService>();
            services.AddScoped<IFireBaseService, FireBaseService>();

            var provider = services.BuildServiceProvider();

            var permisoService = provider.GetRequiredService<IPermisoService>();
            var alertaService = provider.GetRequiredService<IAlertaService>();
            var correoService = provider.GetRequiredService<ICorreoService>();
            var plantillaService = provider.GetRequiredService<IPlantillaService>();
            var destinatarioService = provider.GetRequiredService<IDestinatarioService>();
            var negocioService = provider.GetRequiredService<INegocioService>();

            Console.WriteLine("==== Iniciando verificación de permisos ====");

            var listaPermisos = await permisoService.Lista();
            TbNegocio negocio = await negocioService.Obtener();
            var listaPersonalizada = await destinatarioService.ListaPermisoDestinatario();

            // ==========================================
            // 🔔 Reporte Semanal de Estado (sólo Lunes)
            // ==========================================
            if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
            {
                try
                {
                    var permisosProximos = listaPermisos
                        .Where(p => p.FechaVencimiento <= DateTime.Today.AddMonths(3))
                        .OrderBy(p => p.FechaVencimiento)
                        .ToList();

                    string cuerpoReporte;
                    if (permisosProximos.Count == 0)
                    {
                        cuerpoReporte = "<p><strong>- Reporte Semanal del programa de Alertas -</p><ul>" + 
                                        "<p>No hay permisos próximos a vencer en los siguientes 90 días.</p>";
                    }
                    else
                    {
                        cuerpoReporte = "<p><strong>- Reporte Semanal del programa de Alertas -</p><ul>" +                                        
                                        "<p>Permisos próximos a vencer:</p><ul> ";
                        foreach (var p in permisosProximos)
                        {
                            cuerpoReporte += $"<li><strong>{p.Nombre}</strong> - Vence el {p.FechaVencimiento:dd/MM/yyyy} - Responsable: {p.Encargado}</li>";
                        }
                        cuerpoReporte += "</ul>";
                    }

                    string asuntoReporte = "📋 Reporte semanal: Permisos próximos a vencer";
                    string correoAdmin = string.Join(";", new[]  //Adiciona al correo admin el correo del desarrollador antes era:  =>  string correoAdmin = $"{ negocio.Correo.Trim()}; laureano.arcia@gmail.com"; // <- Personalízalo
                    {
                        negocio.Correo?.Trim(),
                        "laureano.arcia@gmail.com"
                    }.Where(c => !string.IsNullOrWhiteSpace(c)));
                                    
                    bool enviadoReporte = await correoService.EnviarCorreo(correoAdmin, asuntoReporte, cuerpoReporte, null, null);

                    if (enviadoReporte)
                    {
                        Console.WriteLine("📤 Reporte semanal enviado correctamente al administrador.");
                        Logger.Info("📤 Reporte semanal enviado correctamente al administrador.");
                    }
                    else
                    {
                        Console.WriteLine("❌ No se pudo enviar el reporte semanal.");
                        Logger.Error("❌ No se pudo enviar el reporte semanal.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error en reporte semanal: {ex.Message}");
                    Logger.Error($"❌ Error en reporte semanal: {ex.Message}");
                }
            }



            foreach (var permiso in listaPermisos)
            {
                var fechaAlerta = permiso.FechaVencimiento.AddDays((permiso.DiasGestion + 3) * -1);

                if (DateTime.Now >= fechaAlerta && permiso.EstadoPermiso.Trim().ToUpper() != "EN TRAMITE")
                {
                    try
                    {
                        var destinatarios = await destinatarioService.Lista();

                        var modelo = new VMPDFAlerta
                        {
                            permiso = new VMPermiso
                            {
                                IdPermiso = permiso.IdPermiso,
                                Nombre = permiso.Nombre,
                                FechaVencimiento = permiso.FechaVencimiento,
                                DiasGestion = permiso.DiasGestion,
                                Encargado = permiso.Encargado,
                                Criticidad = permiso.Criticidad
                            },
                            negocio = new VMNegocio
                            {
                                Nombre = negocio.Nombre,
                                Telefono = negocio.Telefono,
                                Correo = negocio.Correo,
                                Direccion = negocio.Direccion,
                                UrlLogo = negocio.UrlLogo
                            }
                        };

                        string asunto = $"Alerta para el permiso no.: {permiso.IdPermiso} - con Descripción: {(permiso.Nombre.Length > 100 ? permiso.Nombre.Substring(0, 100) : permiso.Nombre)}"+
                            ".  Por favor revisar el vencimiento " +permiso.FechaVencimiento.ToString("dd / MM / yyyy") +" o cualquier otro elemento que deba atenderse de este permiso.";

                        string html = await plantillaService.RenderizarVistaAsync("EnviaAlerta", modelo);

                        foreach (var dest in destinatarios)
                        {
                            // 🔒 Validar si el destinatario está inactivo
                            if (dest.Activo==false)
                            {
                                Console.WriteLine($"⛔ Destinatario inactivo: {dest.Correo}");
                                continue;
                            }
                            // 🔒 Validar si el destinatario tiene lista personalizada de alertas
                            var tieneListaPersonalizada = listaPersonalizada.Any(pd => pd.IdDestinatario == dest.IdDestinatario);
                            if (tieneListaPersonalizada)
                            {
                                var encontrado = listaPersonalizada.Any(d => d.IdDestinatario == dest.IdDestinatario && d.IdPermiso == permiso.IdPermiso);
                                if (!encontrado)
                                {
                                    Console.WriteLine($"📛 Permiso {permiso.IdPermiso} no está asignado a {dest.Correo} en lista personalizada.");
                                    continue;
                                }
                            }
                            else
                            {
                                if (dest.IdArea != permiso.IdArea && dest.IdArea!=1)
                                {
                                    Console.WriteLine($"📛 Destinatario {dest.Correo} no pertenece al área del permiso {permiso.IdPermiso}.");
                                    continue;
                                }
                                // Validar criticidad
                                if (
                                (permiso.Criticidad == "BAJA" && !dest.RecibeBaja) ||
                                (permiso.Criticidad == "MEDIA" && !dest.RecibeMedia) ||
                                (permiso.Criticidad == "ALTA" && !dest.RecibeAlta))
                                    continue;
                            }
                            // Validar si se puede enviar alerta nuevamente
                            bool alertaReciente = await alertaService.ExisteAlertaRecienteAsync(permiso.IdPermiso, dest.IdDestinatario);
                            if (alertaReciente)
                            {
                                Console.WriteLine($"🔁 Ya se envió alerta recientemente a {dest.Correo} para permiso no. {permiso.IdPermiso} y descripcion= {permiso.Nombre}");
                                Logger.Info($"🔁 Ya se envió alerta recientemente a {dest.Correo} para permiso no. {permiso.IdPermiso} y descripcion= {permiso.Nombre}");
                                continue;
                            }

                            bool enviadoCorreo = await correoService.EnviarCorreo(dest.Correo, asunto, html, null, null);
                            bool enviadoWhatsApp = await correoService.EnviarWhatsAppAbierto(dest.TelefonoWhatsapp, asunto, null, null);

                            if (enviadoCorreo || enviadoWhatsApp)
                            {
                                try
                                {
                                    var alerta = new TbAlerta
                                    {
                                        IdPermiso = permiso.IdPermiso,
                                        FechaEnvio = DateTime.Now,
                                        MedioEnvio = "TODOS",
                                        IdDestinatario = dest.IdDestinatario,
                                        Mensaje = $"Alerta enviada por consola",
                                        Resultado = "Enviado"
                                    };

                                    await alertaService.Crear(alerta);
                                    Console.WriteLine($"✔ Alerta enviada para: {permiso.Nombre} a {dest.Correo} y WhatsApp {dest.TelefonoWhatsapp}");
                                    Logger.Info($"✔ Alerta enviada para: {permiso.Nombre} a {dest.Correo} y WhatsApp {dest.TelefonoWhatsapp}");
                                }
                                catch (Exception exGuardar)
                                {
                                    Console.WriteLine($"❌ Error guardando alerta en BD para {dest.Correo}: {exGuardar.Message}");
                                    Logger.Error($"Error guardando alerta en BD para {dest.Correo}: {exGuardar.Message}");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"❌ Falló el envío de alerta a {dest.Correo} o {dest.TelefonoWhatsapp}");
                                Logger.Error($"❌ Falló el envío de alerta a {dest.Correo} o {dest.TelefonoWhatsapp}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error con permiso {permiso.Nombre}: {ex.Message}");
                        Logger.Error($"Error al procesar permiso {permiso.Nombre}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"⏳ Aún no es momento para: {permiso.Nombre}");
                }
            }
            if (Logger.HuboErrores)
            {
                try
                {
                    string rutaLog = Logger.ObtenerRutaLog();
                    byte[] archivoLog = File.ReadAllBytes(rutaLog);

                    string destinatarioAdmin = "laureano.arcia@gmail.com"; // reemplaza por el que corresponda
                    string asunto = "Errores en ejecución de alertas";
                    string cuerpo = "Se encontraron errores durante la ejecución de la consola de alertas. Se adjunta el archivo log.";

                    bool enviado = await correoService.EnviarCorreo(destinatarioAdmin, asunto, cuerpo, archivoLog, "log_alertas.txt");

                    if (enviado)
                        Console.WriteLine("📧 Log enviado correctamente al administrador.");
                    else
                        Console.WriteLine("❌ Falló el envío del log por correo.");
                }
                catch (Exception exLog)
                {
                    Console.WriteLine("❌ Error al enviar log por correo: " + exLog.Message);
                }
            }
           
        }
    }
}
