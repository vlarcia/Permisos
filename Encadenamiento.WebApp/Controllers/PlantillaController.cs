using AutoMapper;
using Business.Interfaces;
using Encadenamiento.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
        private readonly IPlanesTrabajoService _planService;
        public PlantillaController(IMapper mapper,
            INegocioService negocioService,
            IPlanesTrabajoService planService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
            _planService = planService;

        }

        [AllowAnonymous]
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"]=correo;
            ViewData["Clave"]=clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }
        

        [AllowAnonymous]
        public IActionResult RestablecerClave(string clave   )
        {
            ViewData["Clave"] = clave;
            return View();
        }
    }
}
