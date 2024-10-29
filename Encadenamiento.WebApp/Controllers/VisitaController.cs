using AutoMapper;
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
            List<VMVisita> vmPlanesLista = _mapper.Map<List<VMPVisita>>(await _visitaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmPlanesLista });
        }
    }
}
