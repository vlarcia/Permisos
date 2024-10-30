using AutoMapper;
using Business.Implementacion;
using Business.Interfaces;
using Encadenamiento.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMVisita> vmVisitaLista = _mapper.Map<List<VMVisita>>(await _visitaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmVisitaLista });
        }
    }
}
