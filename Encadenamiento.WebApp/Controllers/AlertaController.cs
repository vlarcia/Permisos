using AutoMapper;
using Business.Implementacion;
using Business.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using OpenQA.Selenium;
using Permisos.WebApp.Models.ViewModels;
using Permisos.WebApp.Utilidades.Response;
using Rotativa.AspNetCore;
using System.ComponentModel;

namespace Permisos.WebApp.Controllers
{
    public class AlertaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAlertaService _alertaService;
        private readonly IPermisoService _permisoService;
        private readonly IDestinatarioService _destinatarioService;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly ICorreoService _correoService;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly INegocioService _negocioService;

        public AlertaController(IAlertaService alertaService, IPermisoService permisoService,
            IDestinatarioService destinatarioService, ICorreoService correoService, IMapper mapper,
            ITempDataProvider tempDataProvider, IRazorViewEngine razorViewEngine, INegocioService negocioService)
        {
            _alertaService = alertaService;
            _permisoService = permisoService; 
            _destinatarioService = destinatarioService;
            _mapper = mapper;
            _correoService = correoService;
            _tempDataProvider = tempDataProvider;
            _razorViewEngine = razorViewEngine;
            _negocioService = negocioService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Mensajeria()
        {
            return View();
        }
        public IActionResult Whatsapp()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListaPermisos()
        {
            List<VMAlerta> VMAlertaLista = _mapper.Map<List<VMAlerta>>(await _alertaService.Lista());
            return StatusCode(StatusCodes.Status200OK, VMAlertaLista);

        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMAlerta> VMAlertaLista = _mapper.Map<List<VMAlerta>>(await _alertaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = VMAlertaLista });
        }
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMAlerta> gResponse = new GenericResponse<VMAlerta>();
            try

            {
                VMAlerta VMAlerta = JsonConvert.DeserializeObject<VMAlerta>(modelo);

                TbAlerta permiso_creado = await _alertaService.Crear(_mapper.Map<TbAlerta>(VMAlerta));
                VMAlerta = _mapper.Map<VMAlerta>(permiso_creado);
                gResponse.Estado = true;
                gResponse.Objeto = VMAlerta;

            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            GenericResponse<VMAlerta> gResponse = new GenericResponse<VMAlerta>();
            try
            {
                VMAlerta VMAlerta = JsonConvert.DeserializeObject<VMAlerta>(modelo);

                TbAlerta permiso_editado = await _alertaService.Editar(_mapper.Map<TbAlerta>(VMAlerta));

                VMAlerta = _mapper.Map<VMAlerta>(permiso_editado);
                gResponse.Estado = true;
                gResponse.Objeto = VMAlerta;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int idAlerta)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _alertaService.Eliminar(idAlerta);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarWhatsAppAbierto(IFormFile archivo, string destino, string mensaje)
        {
            try
            {
                Stream streamArchivo = null;
                string nombreArchivo = null;

                if (archivo != null && archivo.Length > 0)
                {
                    streamArchivo = archivo.OpenReadStream();
                    nombreArchivo = archivo.FileName;
                }

                bool exito = await _correoService.EnviarWhatsAppAbierto(destino, mensaje, streamArchivo, nombreArchivo);
                return Json(new { exito });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = ex.Message });
            }
        }
        public async Task<VMPDFAlerta> PDFAlerta(int idPermiso)
        {
            VMPermiso vmPermiso = _mapper.Map<VMPermiso>(await _permisoService.ObtenerPorId(idPermiso));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            VMPDFAlerta modelo = new VMPDFAlerta();
            modelo.negocio = vmNegocio;
            modelo.permiso = vmPermiso;

            return modelo;
        }
        public async Task<IActionResult> EnviarAlerta(int idPermiso, int wasap)
        {
            try
            {
                VMPDFAlerta modelo = await PDFAlerta(idPermiso);    // Obtener el modelo 
                TbPermiso permiso_enviar = await _permisoService.ObtenerPorId(idPermiso);  // Obtener el permiso por id
                List<TbDestinatario> destinatarios = await _destinatarioService.Lista(); // Obtener la lista de destinatarios

                var listaPersonalizada= await _destinatarioService.ListaPermisoDestinatario(); // Obtener la lista personalizada de alertas de destinatario

                string asunto = $"Alerta para el permiso no.: {permiso_enviar.IdPermiso} - y Descripción :  {(permiso_enviar.Nombre.Length > 100 ? permiso_enviar.Nombre.Substring(0, 100) : permiso_enviar.Nombre)}";

                string htmlCorreo = await RenderViewAsString("EnviaALerta", modelo); // Generar HTML de la plantilla
                if (wasap != 1)
                {
                    foreach (var destinatario in destinatarios)
                    {
                        if (!string.IsNullOrWhiteSpace(destinatario.Correo))
                        {
                            if (destinatario.Activo == false)
                            {
                                continue;
                            }
                            var tieneListaPersonalizada = listaPersonalizada.Any(pd => pd.IdDestinatario == destinatario.IdDestinatario);
                            if (tieneListaPersonalizada)
                            {
                                var encontrado = listaPersonalizada.Any(d => d.IdDestinatario == destinatario.IdDestinatario && d.IdPermiso == permiso_enviar.IdPermiso);
                                if (!encontrado)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (destinatario.IdArea != permiso_enviar.IdArea && destinatario.IdArea!=1)
                                {
                                    continue;
                                }

                                // Si NO cumple con la criticidad, saltar al siguiente destinatario
                                if (
                                    (permiso_enviar.Criticidad == "BAJA" && !destinatario.RecibeBaja) ||
                                    (permiso_enviar.Criticidad == "MEDIA" && !destinatario.RecibeMedia) ||
                                    (permiso_enviar.Criticidad == "ALTA" && !destinatario.RecibeAlta)
                                )                           
                                continue;
                            }

                            bool correoEnviado = await _correoService.EnviarCorreo(destinatario.Correo, asunto, htmlCorreo, null, null);
                            if (correoEnviado)
                            {
                                TbAlerta alertaenviada = new TbAlerta
                                {
                                    IdPermiso = idPermiso,
                                    FechaEnvio = DateTime.Now,
                                    MedioEnvio = "CORREO",
                                    IdDestinatario = destinatario.IdDestinatario,
                                    Mensaje = $"Alerta de permiso {permiso_enviar.IdPermiso} - {permiso_enviar.Nombre}",
                                    Resultado = "Enviado",
                                };
                                await _alertaService.Crear(alertaenviada, permitirDuplicado: true); // Guardar la alerta enviada en la base de datos

                            }
                            else
                            {
                                return BadRequest(new { estado = false, mensaje = "Error al enviar la alerta a un destinatario" });
                            }
                        }
                        ;

                    }
                    return Ok(new { estado = true });
                }
                else
                {
                    foreach (var destinatario in destinatarios)
                    {
                        if (!string.IsNullOrWhiteSpace(destinatario.TelefonoWhatsapp))
                        {
                            asunto = asunto + ".  Por favor revise el vencimiento " +permiso_enviar.FechaVencimiento.ToString("dd/MM/yyyy") +" o cualquier otro elemento que debe atender de este permiso.";

                            bool watsap_enviado = await _correoService.EnviarWhatsApp(destinatario.TelefonoWhatsapp, asunto, null, null);
                            if (watsap_enviado)
                            {
                                TbAlerta alertaenviada = new TbAlerta
                                {
                                    IdPermiso = idPermiso,
                                    FechaEnvio = DateTime.Now,
                                    MedioEnvio = "WHATSAPP",
                                    IdDestinatario = destinatario.IdDestinatario,
                                    Mensaje = $"Alerta de permiso {permiso_enviar.IdPermiso} - {permiso_enviar.Nombre}",
                                    Resultado = "Enviado",
                                };
                                await _alertaService.Crear(alertaenviada, permitirDuplicado: true); // Guardar la alerta enviada en la base de datos
                            }
                            else
                            {
                                return BadRequest(new { estado = false, mensaje = "Error al enviar el WhatsApp, Revise el numero del destinatario" });
                            }
                        }
                    }
                    return Ok(new { estado = true });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { estado = false, mensaje = ex.Message });
            }
        }
        public async Task<string> RenderViewAsString(string viewName, object model)
        {
            // Intentar encontrar la vista
            var viewResult = _razorViewEngine.FindView(ControllerContext, viewName, false);

            // Si no se encuentra la vista, lanzar una excepción
            if (!viewResult.Success)
            {
                throw new ArgumentNullException($"View {viewName} not found");
            }

            // Crear un ViewDataDictionary con el modelo correcto
            var viewDataDictionary = new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model // Aquí se asegura que el modelo correcto se pase
            };

            using (var sw = new StringWriter())
            {
                // Crear el ViewContext
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDataDictionary,
                    new TempDataDictionary(ControllerContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                // Renderizar la vista
                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}
