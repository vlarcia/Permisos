using AutoMapper;
using Business.Interfaces;
using Permisos.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Permisos.WebApp.Controllers
{
    [Authorize]
    public class PlantillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
    
        public PlantillaController(IMapper mapper,
            INegocioService negocioService)
        {
            _mapper = mapper;
            _negocioService = negocioService;
            

        }

        [AllowAnonymous]
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"]=correo;
            ViewData["Clave"]=clave;
            ViewData["Url"] = $"{Request.Scheme}://{Request.Host}";
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
