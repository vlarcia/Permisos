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

        [HttpGet]
        public async Task<IActionResult> ObtenerCumplimientoGeneral(int idFinca, string fecha, int grupo)
        {
            // Obtener la lista de revisiones del servicio
            var revisiones = await _revisionService.ObtenerRevision(idFinca, fecha, grupo);

            // Mapeo a VMRevisiones
            var vmRevisiones = _mapper.Map<List<VMRevisiones>>(revisiones);

            // Contar cada estado de cumplimiento
            int totalCumple = vmRevisiones.Count(r => r.Estado == "CUMPLE");
            int totalParcial = vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL");
            int totalNoCumple = vmRevisiones.Count(r => r.Estado == "NO CUMPLE");
            int totalNoAplica = vmRevisiones.Count(r => r.Estado == "NO APLICA");

            int totalLaboral = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "LABORAL");
            int totalOcupacional = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "OCUPACIONAL");
            int totalAmbiental = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "AMBIENTAL");
            int totalRse = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "RSE");



            int totalgeneral =totalCumple+totalParcial+totalNoCumple+totalNoAplica;
            decimal porcentCumple = (decimal)totalCumple / totalgeneral *100;
            decimal porcentParcial=(decimal)totalParcial / totalgeneral *100;
            decimal porcentNoCumple=(decimal)totalNoCumple / totalgeneral * 100;
            decimal porcentNoAplica=(decimal)totalNoAplica / totalgeneral * 100;

            decimal porcLaboral=(vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "LABORAL")+ (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "LABORAL")*0.5m))/totalLaboral *100;
            decimal porcOcupacional = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "OCUPACIONAL") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "OCUPACIONAL") * 0.5m)) / totalOcupacional *100;
            decimal porcAmbiental =(vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "AMBIENTAL") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "AMBIENTAL") * 0.5m)) / totalAmbiental * 100;
            decimal porcRse = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "RSE") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "RSE") * 0.5m)) / totalRse * 100;



            // Calcular el porcentaje de cumplimiento general
            decimal cumplimientoGeneral = (totalCumple + (totalParcial * 0.5m)) / (totalCumple + totalParcial + totalNoCumple) * 100;

            // Crear el objeto de respuesta con los datos necesarios
            var respuesta = new
            {
                CumplimientoGeneral = cumplimientoGeneral,
                estado=true,
                Estados = new
                {
                    Cumple = totalCumple,
                    Parcial = totalParcial,
                    NoCumple = totalNoCumple,
                    NoAplica = totalNoAplica,

                    porcCumple=Math.Round(porcentCumple,2),
                    porcParcial= Math.Round(porcentParcial,2),                    
                    porcNoCumple= Math.Round(porcentNoCumple, 2),
                    porcNoAplica = Math.Round(porcentNoAplica, 2),

                    porcLaboral = Math.Round(porcLaboral, 2),
                    porcOcupacional = Math.Round(porcOcupacional, 2),
                    porcAmbiental = Math.Round(porcAmbiental, 2),
                    porcRse = Math.Round(porcRse, 2),
                },
              
            };

            return StatusCode(StatusCodes.Status200OK, respuesta);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCumplimientoxFinca(int idFinca)
        {
            // Obtener todas las revisiones de la finca, ordenadas por fecha
            var revisiones = await _revisionService.ObtenerRevision(idFinca, "", 0);

            // Agrupa las revisiones por fecha
            var revisionesAgrupadas = revisiones
                .GroupBy(r => r.Fecha)
                .OrderBy(g => g.Key) // Ordena por fecha para mantener el orden cronológico
                .ToList();

            // Lista para almacenar los datos de cada fecha
            var seriesDeCumplimiento = new List<object>();

            // Procesa cada grupo de revisiones (cada fecha)
            foreach (var grupoRevisiones in revisionesAgrupadas)
            {
                var vmRevisiones = _mapper.Map<List<VMRevisiones>>(grupoRevisiones);

                int totalCumple = vmRevisiones.Count(r => r.Estado == "CUMPLE");
                int totalParcial = vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL");
                int totalNoCumple = vmRevisiones.Count(r => r.Estado == "NO CUMPLE");
                int totalNoAplica = vmRevisiones.Count(r => r.Estado == "NO APLICA");

                int totalLaboral = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "LABORAL");
                int totalOcupacional = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "OCUPACIONAL");
                int totalAmbiental = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "AMBIENTAL");
                int totalRse = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "RSE");

                int totalGeneral = totalCumple + totalParcial + totalNoCumple + totalNoAplica;
                decimal porcentCumple = (decimal)totalCumple / totalGeneral * 100;
                decimal porcentParcial = (decimal)totalParcial / totalGeneral * 100;
                decimal porcentNoCumple = (decimal)totalNoCumple / totalGeneral * 100;
                decimal porcentNoAplica = (decimal)totalNoAplica / totalGeneral * 100;

                decimal porcLaboral = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "LABORAL") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "LABORAL") * 0.5m)) / totalLaboral * 100;
                decimal porcOcupacional = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "OCUPACIONAL") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "OCUPACIONAL") * 0.5m)) / totalOcupacional * 100;
                decimal porcAmbiental = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "AMBIENTAL") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "AMBIENTAL") * 0.5m)) / totalAmbiental * 100;
                decimal porcRse = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "RSE") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "RSE") * 0.5m)) / totalRse * 100;

                decimal cumplimientoGeneral = (totalCumple + (totalParcial * 0.5m)) / (totalCumple + totalParcial + totalNoCumple) * 100;

                // Agregar datos de esta fecha a la serie de cumplimiento
                seriesDeCumplimiento.Add(new
                {
                    Fecha = (grupoRevisiones.Key).ToString("dd/MM/yyyy"),
                    CumplimientoGeneral = cumplimientoGeneral,                
                    Cumple = totalCumple,
                    Parcial = totalParcial,
                    NoCumple = totalNoCumple,
                    NoAplica = totalNoAplica,
                    porcCumple = Math.Round(porcentCumple, 2),
                    porcParcial = Math.Round(porcentParcial,2),
                    porcNoCumple = Math.Round(porcentNoCumple,2),
                    porcNoAplica = Math.Round(porcentNoAplica,2),
                    porcLaboral = Math.Round(porcLaboral,2),
                    porcOcupacional = Math.Round(porcOcupacional,2),
                    porcAmbiental = Math.Round(porcAmbiental,2),
                    porcRse = Math.Round(porcRse,2),
                });
            }

            // Devolver el resultado con estado = true y la lista de series de cumplimiento
            return StatusCode(StatusCodes.Status200OK, new { estado = true, SeriesDeCumplimiento = seriesDeCumplimiento });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCumplimientoxGrupo(int grupo)
        {
            // Obtener todas las revisiones del grupo seleccionado
            var revisiones = await _revisionService.ObtenerRevision(0, "", grupo);           

            // Agrupar las revisiones por tipo (INICIAL, INTERMEDIO, FINAL)
            var revisionesAgrupadasPorTipo = revisiones
                .GroupBy(r => r.Tipo) // Agrupar por tipo (INICIAL, INTERMEDIO, FINAL)
                .OrderBy(g => g.Key) // Ordenar por tipo para mantener el orden
                .ToList();

            // Lista para almacenar los datos de cada tipo
            var seriesDeCumplimiento = new List<object>();

            // Procesar cada grupo de revisiones (por tipo)
            foreach (var grupoRevisiones in revisionesAgrupadasPorTipo)
            {
                var vmRevisiones = _mapper.Map<List<VMRevisiones>>(grupoRevisiones);

                int totalCumple = vmRevisiones.Count(r => r.Estado == "CUMPLE");
                int totalParcial = vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL");
                int totalNoCumple = vmRevisiones.Count(r => r.Estado == "NO CUMPLE");
                int totalNoAplica = vmRevisiones.Count(r => r.Estado == "NO APLICA");

                int totalLaboral = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "LABORAL");
                int totalOcupacional = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "OCUPACIONAL");
                int totalAmbiental = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "AMBIENTAL");
                int totalRse = vmRevisiones.Count(r => r.Estado != "NO APLICA" && r.Ambito == "RSE");

                int totalGeneral = totalCumple + totalParcial + totalNoCumple + totalNoAplica;
                decimal porcentCumple = (decimal)totalCumple / totalGeneral * 100;
                decimal porcentParcial = (decimal)totalParcial / totalGeneral * 100;
                decimal porcentNoCumple = (decimal)totalNoCumple / totalGeneral * 100;
                decimal porcentNoAplica = (decimal)totalNoAplica / totalGeneral * 100;

                decimal porcLaboral = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "LABORAL") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "LABORAL") * 0.5m)) / totalLaboral * 100;
                decimal porcOcupacional = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "OCUPACIONAL") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "OCUPACIONAL") * 0.5m)) / totalOcupacional * 100;
                decimal porcAmbiental = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "AMBIENTAL") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "AMBIENTAL") * 0.5m)) / totalAmbiental * 100;
                decimal porcRse = (vmRevisiones.Count(r => r.Estado == "CUMPLE" && r.Ambito == "RSE") + (vmRevisiones.Count(r => r.Estado == "CUMPLE PARCIAL" && r.Ambito == "RSE") * 0.5m)) / totalRse * 100;

                decimal cumplimientoGeneral = (totalCumple + (totalParcial * 0.5m)) / (totalCumple + totalParcial + totalNoCumple) * 100;

                // Agregar datos de este tipo a la serie de cumplimiento
                seriesDeCumplimiento.Add(new
                {
                    Tipo = grupoRevisiones.Key, // Tipo de revisión (INICIAL, INTERMEDIO, FINAL)
                    CumplimientoGeneral = cumplimientoGeneral,
                    Cumple = totalCumple,
                    Parcial = totalParcial,
                    NoCumple = totalNoCumple,
                    NoAplica = totalNoAplica,
                    porcCumple = Math.Round(porcentCumple, 2),
                    porcParcial = Math.Round(porcentParcial, 2),
                    porcNoCumple = Math.Round(porcentNoCumple, 2),
                    porcNoAplica = Math.Round(porcentNoAplica, 2),
                    porcLaboral = Math.Round(porcLaboral, 2),
                    porcOcupacional = Math.Round(porcOcupacional, 2),
                    porcAmbiental = Math.Round(porcAmbiental, 2),
                    porcRse = Math.Round(porcRse, 2),
                });
            }

            // Devolver el resultado con estado = true y la lista de series de cumplimiento
            return StatusCode(StatusCodes.Status200OK, new { estado = true, SeriesDeCumplimiento = seriesDeCumplimiento });
        }

    }
}
