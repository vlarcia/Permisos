using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Encadenamiento.WebApp.Models.ViewModels;
using Encadenamiento.WebApp.Utilidades.Response;
using Business.Interfaces;
using Entity;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Collections.Immutable;
using Business.Implementacion;
using Data.Interfaces;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Encadenamiento.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Html;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;


namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class PlanesTrabajoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPlanesTrabajoService _plantrabajoService;        
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly INegocioService _negocioService;
        private readonly ICorreoService _correoService;


        public PlanesTrabajoController(IPlanesTrabajoService plantrabajoService, IMapper mapper,
                                        INegocioService negocioService, ICorreoService CorreoService,
                                        IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider)
        {
            _plantrabajoService = plantrabajoService;
            _mapper = mapper;
            
            //_razorViewEngine = razorViewEngine;
            //_tempDataProvider = tempDataProvider;
            _negocioService = negocioService;
            _correoService = CorreoService;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Planes()
        {
            return View();
        }
        public IActionResult Actividades()
        {
            return View();
        }

  
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMPlanesTrabajo> vmPlanesLista = _mapper.Map<List<VMPlanesTrabajo>>(await _plantrabajoService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmPlanesLista });
        }


        [HttpGet]
        public async Task<IActionResult> ListaActividad()
        {
            List<VMActividad> vmActividadesLista = _mapper.Map<List<VMActividad>>(await _plantrabajoService.ListaActividad());
            return StatusCode(StatusCodes.Status200OK, new { data = vmActividadesLista });
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarPlan([FromBody] VMPlanesTrabajo modelo)
        {
            GenericResponse<VMPlanesTrabajo> gResponse = new GenericResponse<VMPlanesTrabajo>();
            try
            {                
                PlanesTrabajo plan_creado = await _plantrabajoService.Registrar(_mapper.Map<PlanesTrabajo>(modelo));
                modelo = _mapper.Map<VMPlanesTrabajo>(plan_creado);
                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


        [HttpPost]
        public async Task<IActionResult> RegistrarActividad([FromBody] VMActividad modelo)
        {
            GenericResponse<VMActividad> gResponse = new GenericResponse<VMActividad>();
            try

            {
                Actividad act_creado = await _plantrabajoService.RegistrarActividad(_mapper.Map<Actividad>(modelo));
                modelo = _mapper.Map<VMActividad>(act_creado);
                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPlan([FromBody] VMPlanesTrabajo modelo)
        {
            GenericResponse<VMPlanesTrabajo> gResponse = new GenericResponse<VMPlanesTrabajo>();
            try
            {
                PlanesTrabajo plan_editado = await _plantrabajoService.Editar(_mapper.Map<PlanesTrabajo>(modelo));
                modelo = _mapper.Map<VMPlanesTrabajo>(plan_editado);
                gResponse.Estado = true;
                gResponse.Objeto = modelo;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


        [HttpPost]
        public async Task<IActionResult> EditarActividad([FromBody] VMActividad modelo)
        {
            GenericResponse<VMActividad> gResponse = new GenericResponse<VMActividad>();
            try
            {
                Actividad act_editada = await _plantrabajoService.EditarActividad(_mapper.Map<Actividad>(modelo));
                modelo = _mapper.Map<VMActividad>(act_editada);
                gResponse.Estado = true;
                gResponse.Objeto = modelo;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


        [HttpGet]
        public async Task<IActionResult> Historial(int IdPlan, string fechainicio, string fechafin)
        {
            List<VMPlanesTrabajo> vmHistorialVenta = _mapper.Map<List<VMPlanesTrabajo>>(await _plantrabajoService.Historial(IdPlan, fechainicio, fechafin));
            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }


        [HttpPost]
        public async Task<IActionResult> Eliminar(int idPlan)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _plantrabajoService.Eliminar(idPlan);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarActividad(int idActividad)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _plantrabajoService.EliminarActividad(idActividad);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        public async Task<VMPDFPlanesTrabajo> PDFPlanModelo(int idPlan)
        {
            VMPlanesTrabajo vmPlan = _mapper.Map<VMPlanesTrabajo>(await _plantrabajoService.Detalle(idPlan));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            VMPDFPlanesTrabajo modelo = new VMPDFPlanesTrabajo();
            modelo.negocio = vmNegocio;
            modelo.plan = vmPlan;

            return modelo;

        }
        public async Task<IActionResult> PDFPlan(int idPlan)
        {
            VMPlanesTrabajo vmPlan = _mapper.Map<VMPlanesTrabajo>(await _plantrabajoService.Detalle(idPlan));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            VMPDFPlanesTrabajo modelo = new VMPDFPlanesTrabajo();
            modelo.negocio = vmNegocio;
            modelo.plan = vmPlan;

            return View(modelo);
        }
        public async Task<IActionResult> MostrarPDFPlan(int idPlan)
        {
            // Llamar al método para obtener el modelo
            var modelo = await PDFPlanModelo(idPlan);

            // Convertir y devolver el PDF
            return new ViewAsPdf("PDFPlan", modelo)
            {
                FileName = $"Plan_{idPlan}.pdf",
                CustomSwitches = "--no-print-media-type --viewport-size 1280x1024", // Opciones adicionales para control
                ContentDisposition = Rotativa.AspNetCore.Options.ContentDisposition.Inline // Mostrar en lugar de descargar
            };
        }

        public async Task<IActionResult> EnviarPlanPorCorreo(int idPlan, string correoDestino)
        {
            try
            {
                // Obtener el modelo del plan
                VMPDFPlanesTrabajo modelo = await PDFPlanModelo(idPlan);

                // Crear el PDF
                var pdf = new ViewAsPdf("PDFPlan", modelo);           
                
                // Convertir el PDF a un byte array
                byte[] archivoPDF = await pdf.BuildFile(ControllerContext);

                // Asunto y mensaje del correo
                string asunto = "Plan de Trabajo para la finca - " + modelo.plan.NombreFinca;             

                // Obtener la plantilla del correo               
                string htmlCorreo = await RenderViewAsString("EnviaPlan", modelo); // Generar HTML de la plantilla

                // Enviar el correo
                bool correoEnviado = await _correoService.EnviarCorreo(correoDestino, asunto, htmlCorreo, archivoPDF, "PlanTrabajo_" + modelo.plan.CodFinca.ToString() + ".pdf");

                if (correoEnviado)
                {
                    return Ok(new { estado = true });
                }
                else
                {
                    return BadRequest(new { estado = false, mensaje = "Error al enviar el correo" });
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



//public IActionResult MostrarPDFPlan(int idPlan)
//{
//    string urlPlantillaVista = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/PDFPlan?idPlan={idPlan}";


//    var pdf = new HtmlToPdfDocument()
//    {
//        GlobalSettings = new GlobalSettings()
//        {
//            PaperSize = PaperKind.Letter,
//            Orientation = Orientation.Portrait,
//        },
//        Objects = {
//             new ObjectSettings(){
//                Page= urlPlantillaVista,
//             }
//        }
//    };
//    var archivoPDF = _converter.Convert(pdf);
//    return File(archivoPDF, "application/pdf");
//}
