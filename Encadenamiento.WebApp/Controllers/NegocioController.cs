using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Business.Interfaces;
using Entity;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Permisos.WebApp.Utilidades.Response;
using Permisos.WebApp.Models.ViewModels;

namespace Permisos.WebApp.Controllers
{
    [Authorize]
    public class NegocioController : Controller
    {
     
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;

        public NegocioController(IMapper mapper, INegocioService negocioService)
        {
            _negocioService = negocioService;
            _mapper = mapper;   
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();
            try
            {
                VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());
                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje=ex.Message;                   
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm]IFormFile logo, [FromForm]string modelo)
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();
            try
            {
                VMNegocio vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);
                string nombreLogo = "";
                Stream logoStream = null;
                if (logo != null)
                {
                    string nombre_en_codigo=Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo=string.Concat(nombre_en_codigo, extension);    
                    logoStream = logo.OpenReadStream();
                }

                TbNegocio negocio_editado= await _negocioService.GuardarCambios(_mapper.Map<TbNegocio>(vmNegocio), logoStream, nombreLogo);
                vmNegocio=_mapper.Map<VMNegocio>(negocio_editado);

                gResponse.Estado = true;
                gResponse.Objeto = vmNegocio;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
