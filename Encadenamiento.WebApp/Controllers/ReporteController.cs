using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Encadenamiento.WebApp.Models.ViewModels;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPlanesTrabajoService _plantrabajoService;
        private readonly IRevisionService _revisionService;

        public ReporteController(IMapper mapper, IPlanesTrabajoService plantrabajoService, IRevisionService revisionService)
        { 
            _mapper = mapper;
            _plantrabajoService = plantrabajoService;
            _revisionService = revisionService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Operativo()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReportePlanes(string fechaInicio, string fechaFin)
        {
            List<VMPlanesTrabajo> vmLista= _mapper.Map<List<VMPlanesTrabajo>>(await _plantrabajoService.Reporte(fechaInicio,fechaFin));
            return StatusCode(StatusCodes.Status200OK, new {Data = vmLista});        
        }
    }
}
