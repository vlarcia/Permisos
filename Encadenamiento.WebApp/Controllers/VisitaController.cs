using AutoMapper;
using Business.Implementacion;
using Business.Interfaces;
using Encadenamiento.WebApp.Models.ViewModels;
using Encadenamiento.WebApp.Utilidades.Response;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Encadenamiento.WebApp.Controllers
{
    public class VisitaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVisitaService _visitaService;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly INegocioService _negocioService;
        private readonly ICorreoService _correoService;
        
        public VisitaController(IVisitaService visitaService, IMapper mapper,
                                       INegocioService negocioService, ICorreoService CorreoService,
                                       IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider)
        {
            _visitaService = visitaService;
            _mapper = mapper;            
            _negocioService = negocioService;
            _correoService = CorreoService;
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Visitas()
        {
            return View();
        }
        public IActionResult Mensajeria()
        {
            return View();
        }

        //[HttpGet]  
        //public async Task<IActionResult> Lista()   // Este era con mapeo
        //{
        //    List<VMVisita> vmVisitaLista = _mapper.Map<List<VMVisita>>(await _visitaService.Lista());
        //    return StatusCode(StatusCodes.Status200OK, new { data = vmVisitaLista });
        //}

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMVisita> vmVisitaLista = _mapper.Map<List<VMVisita>>(await _visitaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmVisitaLista });
        }

        [HttpGet]
        public async Task<IActionResult> ListaDetalle()
        {
            List<VMDetalleVisita> vmVisitaLista = _mapper.Map<List<VMDetalleVisita>>(await _visitaService.ListaDetalle());
            return StatusCode(StatusCodes.Status200OK, new { data = vmVisitaLista });
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromForm] IFormFile foto1, [FromForm] IFormFile foto2, [FromForm] string modeloVisita)
        {
            GenericResponse<VMVisita> gResponse = new GenericResponse<VMVisita>();
            try

            {
                VMVisita vmVisita = JsonConvert.DeserializeObject<VMVisita>(modeloVisita);
                string nombrefoto1 = "";
                string nombrefoto2 = "";
                Stream fotoStream1 = null;
                Stream fotoStream2 = null;
                if (foto1 != null)
                {
                    //string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto1.FileName);
                    nombrefoto1 = string.Concat(vmVisita.IdPlan.ToString(), vmVisita.NombreFoto1, extension);
                    fotoStream1 = foto1.OpenReadStream();
                }
                if (foto2 != null)
                {
                    //string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto2.FileName);
                    nombrefoto2 = string.Concat(vmVisita.IdPlan.ToString(), vmVisita.NombreFoto2, extension);
                    fotoStream2 = foto2.OpenReadStream();
                }
                

                Visita visita_creada = await _visitaService.Registrar(_mapper.Map<Visita>(vmVisita), fotoStream1, nombrefoto1, fotoStream2, nombrefoto2);
                vmVisita = _mapper.Map<VMVisita>(visita_creada);                
                gResponse.Estado = true;
                gResponse.Objeto = vmVisita;

            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto1, [FromForm] IFormFile foto2, [FromForm] string modeloVisita)
        {
            GenericResponse<VMVisita> gResponse = new GenericResponse<VMVisita>();
            try
            {
                VMVisita vmVisita = JsonConvert.DeserializeObject<VMVisita>(modeloVisita);
                string nombrefoto1 = "";
                string nombrefoto2 = "";
                Stream fotoStream1 = null;
                Stream fotoStream2 = null;
                if (foto1 != null)
                {
                    //string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto1.FileName);
                    nombrefoto1 = string.Concat(vmVisita.IdPlan.ToString(), vmVisita.NombreFoto1, extension);
                    fotoStream1 = foto1.OpenReadStream();
                }
                if (foto2 != null)
                {
                    //string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto2.FileName);
                    nombrefoto2 = string.Concat(vmVisita.IdPlan.ToString(), vmVisita.NombreFoto2, extension);
                    fotoStream2 = foto2.OpenReadStream();
                }

                Visita visita_editada = await _visitaService.Editar(_mapper.Map<Visita>(vmVisita), fotoStream1, nombrefoto1, fotoStream2, nombrefoto2);
                vmVisita = _mapper.Map<VMVisita>(visita_editada);
                gResponse.Estado = true;
                gResponse.Objeto = vmVisita;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idVisita)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _visitaService.Eliminar(idVisita);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);     
        }

        public async Task<VMPDFVisita> PDFVisitaModelo(int idVisita)
        {
            VMVisita vmVisita = _mapper.Map<VMVisita>(await _visitaService.Detalle(idVisita));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            VMPDFVisita modelo = new VMPDFVisita();
            modelo.negocio = vmNegocio;
            modelo.visita = vmVisita;

            return modelo;

        }
        public async Task<IActionResult> PDFPVisita(int idVisita)
        {
            VMVisita vmVisita = _mapper.Map<VMVisita>(await _visitaService.Detalle(idVisita));
            VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            VMPDFVisita modelo = new VMPDFVisita();
            modelo.negocio = vmNegocio;
            modelo.visita = vmVisita;

            return View(modelo);
        }
        public async Task<IActionResult> MostrarPDFVisita(int idVisita)
        {
            // Llamar al método para obtener el modelo
            var modelo = await PDFVisitaModelo(idVisita);

            // Convertir y devolver el PDF
            return new ViewAsPdf("PDFVisita", modelo)
            {
                FileName = $"Visita_{idVisita}.pdf",
                CustomSwitches = "--no-print-media-type --viewport-size 1280x1024", // Opciones adicionales para control
                ContentDisposition = Rotativa.AspNetCore.Options.ContentDisposition.Inline // Mostrar en lugar de descargar
            };
        }

        public async Task<IActionResult> EnviarVisitaPorCorreo(int idVisita, string correoDestino)
        {
            try
            {                
                VMPDFVisita modelo = await PDFVisitaModelo(idVisita);    // Obtener el modelo 

                var pdf = new ViewAsPdf("PDFVisita", modelo);   // Crear el PDF
                                
                byte[] archivoPDF = await pdf.BuildFile(ControllerContext);   // Convertir el PDF a un byte array
                                
                string asunto = "Informe de Visita a la finca - " + modelo.visita.NombreFinca;   // Asunto y mensaje del correo

                string htmlCorreo = await RenderViewAsString("EnviaVisita", modelo); // Generar HTML de la plantilla

                // Enviar el correo
                bool correoEnviado = await _correoService.EnviarCorreo(correoDestino, asunto, htmlCorreo, archivoPDF,
                                  "Visita_" + modelo.visita.IdVisita.ToString()+"_"+modelo.visita.CodFinca.ToString() + ".pdf");

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


//[HttpGet]
//public async Task<IActionResult> Lista()   //  Esto era para cuando se recibia data Manual
//{
//    var visitaLista = await _visitaService.Lista();
//    return StatusCode(StatusCodes.Status200OK, new { data = visitaLista });
//}