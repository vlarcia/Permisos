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
using Microsoft.AspNetCore.Authorization;
using Business.Implementacion;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Encadenamiento.WebApp.Controllers;
using Microsoft.AspNetCore.Html;
using System.Net;
using System.Text;


namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class RevisionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRevisionService _revisionService;
        private readonly ICheckListService _checklistService;
        private readonly INegocioService _negocioService;
        private readonly ICorreoService _correoService;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public RevisionController(IRevisionService revisionService, IMapper mapper,
                                  ICheckListService checklistService, INegocioService negocioService,
                                  ICorreoService correoService, IRazorViewEngine razorViewEngine,
                                  ITempDataProvider tempDataProvider)
        {
            _revisionService = revisionService;            
            _mapper = mapper;
            _checklistService = checklistService;
            _negocioService = negocioService;
            _correoService = correoService;
            _razorViewEngine = razorViewEngine; 
            _tempDataProvider = tempDataProvider;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Revisiones()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRequisitos()
        {
            List<VMCheckList> vmListaReqs = _mapper.Map<List<VMCheckList>>(await _checklistService.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaReqs);

        }
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMRevisiones> vmRevisionLista = _mapper.Map<List<VMRevisiones>>(await _revisionService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmRevisionLista });
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerRevision(int idFinca, string fecha)
        {
            List<VMRevisiones> vmRevisionfinca = _mapper.Map<List<VMRevisiones>>(await _revisionService.ObtenerRevision(idFinca, fecha));
            return StatusCode(StatusCodes.Status200OK, new { data = vmRevisionfinca });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] List<Revisione> modelo)
        {
            GenericResponse<VMRevisiones> gResponse = new GenericResponse<VMRevisiones>();
            
            if (modelo == null || !modelo.Any())
            {
                gResponse.Estado = false;
                gResponse.Mensaje = "No se enviaron datos para guardar.";
            }
            try
            {                                
                Revisione revision_creada = await _revisionService.Crear(_mapper.Map<List<Revisione>>(modelo));
                VMRevisiones vmRevision = _mapper.Map<VMRevisiones>(revision_creada); //Tendré el ultimo registro de la revision
                gResponse.Estado=true;
                gResponse.Objeto = vmRevision;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] List<Revisione> modelo)
        {
            GenericResponse<VMRevisiones> gResponse = new GenericResponse<VMRevisiones>();

            try
            {
                if (modelo == null || !modelo.Any())
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "No se enviaron datos para editar.";
                }
                else
                {
                    Revisione revision_editada = await _revisionService.Editar(_mapper.Map<List<Revisione>>(modelo));
                    VMRevisiones vmRevision = _mapper.Map<VMRevisiones>(revision_editada);
                    gResponse.Estado = true;
                    gResponse.Objeto = vmRevision;
                }
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar([FromBody] List<int> modelo)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            
            try
            {
                if (modelo == null || !modelo.Any())
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "No se procesaron los registros de revisones a eliminar";
                }
                else
                {
                    gResponse.Estado = await _revisionService.Eliminar(modelo);
                    gResponse.Estado = true;
                    gResponse.Mensaje = "Revision eliminada con exito";
                }
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        public async Task<VMPDFRevision> PDFRevModelo(int idFinca, string fecha)
        {
            List<VMRevisiones> vmRevision = _mapper.Map<List<VMRevisiones>>(await _revisionService.ObtenerRevision(idFinca,fecha));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            VMPDFRevision modelo = new VMPDFRevision();
            modelo.negocio = vmNegocio;
            modelo.revision = vmRevision;

            return modelo;

        }
       
        public async Task<IActionResult> MostrarPDFRevision(int idFinca, string fecha)
        {
            // Llamar al método para obtener el modelo
            var modelo = await PDFRevModelo(idFinca, fecha);

            // Convertir y devolver el PDF
            return new ViewAsPdf("PDFRevision", modelo)
            {
                FileName = $"Revision_{modelo.revision.First().CodFinca}_{fecha}.pdf",
                //PageSize = Rotativa.AspNetCore.Options.Size.Letter,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(15, 10, 12, 10), // márgenes en mm: top, right, bottom, left
                //IsGrayScale = false,
                //CustomSwitches = "--no print-media-type --no-stop-slow-scripts --javascript-delay 1000 --header-spacing 10",
                CustomSwitches = "--no-print-media-type  --viewport-size 1280x1024 --header-spacing 10", // Opciones adicionales para control
                ContentDisposition = Rotativa.AspNetCore.Options.ContentDisposition.Inline // Mostrar en lugar de descargar
            };
        }

        public async Task<IActionResult> EnviarRevisionPorCorreo(int idFinca, string fecha, string correoDestino)
        {
            try
            {
                // Obtener el modelo del plan
                VMPDFRevision modelo = await PDFRevModelo(idFinca, fecha);

                // Crear el PDF
                var pdf = new ViewAsPdf("PDFRevision", modelo);

                // Convertir el PDF a un byte array
                byte[] archivoPDF = await pdf.BuildFile(ControllerContext);

                // Asunto y mensaje del correo
                string asunto = "Revision realizada en la finca - " + modelo.revision.First().NombreFinca;

                // Obtener la plantilla del correo               
                string htmlCorreo = await RenderViewAsString("EnviaRevision", modelo); // Generar HTML de la plantilla

                // Enviar el correo
                bool correoEnviado = await _correoService.EnviarCorreo(correoDestino, asunto, htmlCorreo, archivoPDF, "Revision_" 
                                            + modelo.revision.First().CodFinca.ToString() + "_"
                                            + modelo.revision.First().NombreFinca.ToString() + ".pdf");

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
