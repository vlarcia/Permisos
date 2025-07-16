using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Permisos.WebApp.Models.ViewModels;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("? Servicio de alertas iniciado a las: {time}", DateTimeOffset.Now);

        using (var scope = _serviceProvider.CreateScope())
        {
            var permisoService = scope.ServiceProvider.GetRequiredService<IPermisoService>();
            var alertaService = scope.ServiceProvider.GetRequiredService<IAlertaService>();
            var correoService = scope.ServiceProvider.GetRequiredService<ICorreoService>();
            var plantillaService = scope.ServiceProvider.GetRequiredService<IPlantillaService>();
            var destinatarioService = scope.ServiceProvider.GetRequiredService<IDestinatarioService>();
            var negocioService = scope.ServiceProvider.GetRequiredService<INegocioService>();

            var listaPermisos = await permisoService.Lista();
            var negocio = await negocioService.Obtener();

            foreach (var permiso in listaPermisos)
            {
                var fechaAlerta = permiso.FechaVencimiento.AddDays((permiso.DiasGestion + 3) * -1);
                if (DateTime.Now >= fechaAlerta)
                {
                    var destinatarios = await destinatarioService.Lista();

                    foreach (var dest in destinatarios)
                    {
                        // Aquí podrías validar criticidad, inactividad, alertas duplicadas, etc.
                        if (dest.Activo==false)
                            continue;

                        bool alertaReciente = await alertaService.ExisteAlertaRecienteAsync(permiso.IdPermiso, dest.IdDestinatario);
                        if (alertaReciente)
                            continue;

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

                        string asunto = $"?? Alerta: {permiso.Nombre}";
                        string html = await plantillaService.RenderizarVistaAsync("EnviaAlerta", modelo);

                        bool enviado = await correoService.EnviarCorreo(dest.Correo, asunto, html, null, null);
                        if (enviado)
                        {
                            await alertaService.Crear(new Entity.TbAlerta
                            {
                                IdPermiso = permiso.IdPermiso,
                                IdDestinatario = dest.IdDestinatario,
                                FechaEnvio = DateTime.Now,
                                Mensaje = "Alerta enviada por Worker Service",
                                MedioEnvio = "Correo",
                                Resultado = "OK"
                            });

                            _logger.LogInformation("? Alerta enviada a: {correo}", dest.Correo);
                        }
                    }
                }
            }
        }

        _logger.LogInformation("? Servicio de alertas finalizado a las: {time}", DateTimeOffset.Now);
    }
}
