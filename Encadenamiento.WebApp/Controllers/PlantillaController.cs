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
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"]=correo;
            ViewData["Clave"]=clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }
        //public async Task<IActionResult> PDFPlan(int idPlan)
        //{
        //    VMPlanesTrabajo vmPlan= _mapper.Map<VMPlanesTrabajo>(await _planService.Detalle(idPlan));
        //    VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());
            
        //    VMPDFPlanesTrabajo modelo=new VMPDFPlanesTrabajo();
        //    modelo.negocio = vmNegocio;
        //    modelo.plan = vmPlan;   
            
        //    return View(modelo);
        //}
        public IActionResult RestablecerClave(string clave   )
        {
            ViewData["Clave"] = clave;
            return View();
        }
    }
}
