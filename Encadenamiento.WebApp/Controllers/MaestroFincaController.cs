using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Encadenamiento.WebApp.Models.ViewModels;
using Encadenamiento.WebApp.Utilidades.Response;
using Business.Interfaces;
using Entity;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Business.Implementacion;
using Microsoft.AspNetCore.Authorization;

namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class MaestroFincaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMaestroFincaService _fincaService;
        
        public MaestroFincaController(IMaestroFincaService fincaService,
          
           IMapper mapper)
        {
            _fincaService = fincaService;
        
            _mapper = mapper;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaFincas()
        {
            List<VMMaestroFinca> vmListaFinca = _mapper.Map<List<VMMaestroFinca>>(await _fincaService.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaFinca);

        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMMaestroFinca> vmFincaLista = _mapper.Map<List<VMMaestroFinca>>(await _fincaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmFincaLista });
        }
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMMaestroFinca> gResponse = new GenericResponse<VMMaestroFinca>();
            try

            {
                VMMaestroFinca vmFinca = JsonConvert.DeserializeObject<VMMaestroFinca>(modelo);
                             
                MaestroFinca finca_creada = await _fincaService.Crear(_mapper.Map<MaestroFinca>(vmFinca));
                vmFinca = _mapper.Map<VMMaestroFinca>(finca_creada);
                gResponse.Estado=true;
                gResponse.Objeto = vmFinca;

            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            GenericResponse<VMMaestroFinca> gResponse = new GenericResponse<VMMaestroFinca>();
            try
            {
                VMMaestroFinca vmFinca = JsonConvert.DeserializeObject<VMMaestroFinca>(modelo);                
              
                MaestroFinca finca_editada = await _fincaService.Editar(_mapper.Map<MaestroFinca>(vmFinca));

                vmFinca = _mapper.Map<VMMaestroFinca>(finca_editada);
                gResponse.Estado = true;
                gResponse.Objeto = vmFinca;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idFinca)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _fincaService.Eliminar(idFinca);
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
