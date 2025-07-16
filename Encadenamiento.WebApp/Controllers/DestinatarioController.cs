using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Permisos.WebApp.Models.ViewModels;
using Permisos.WebApp.Utilidades.Response;
using Business.Interfaces;
using Entity;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Business.Implementacion;
using Microsoft.AspNetCore.Authorization;

namespace Permisos.WebApp.Controllers
{
    public class DestinatarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IDestinatarioService _destinatarioService;
        public DestinatarioController(IDestinatarioService destinatarioService, IMapper mapper)
        {
            _destinatarioService = destinatarioService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListaDestinatarios()
        {
            List<VMDestinatario> vmDestinatarioLista = _mapper.Map<List<VMDestinatario>>(await _destinatarioService.Lista());
            return StatusCode(StatusCodes.Status200OK, vmDestinatarioLista);

        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var idAreaClaim = User.Claims.FirstOrDefault(c => c.Type == "IdArea")?.Value;
            int idArea = 0;
            if (!string.IsNullOrEmpty(idAreaClaim))
            {
                int.TryParse(idAreaClaim, out idArea);
            }

            var destinatarios = await _destinatarioService.ListaPorArea(idArea);
            var vmDestinatarios = _mapper.Map<List<VMDestinatario>>(destinatarios);

            return StatusCode(StatusCodes.Status200OK, new { data = vmDestinatarios });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMDestinatario> gResponse = new GenericResponse<VMDestinatario>();
            try

            {
                VMDestinatario vmFinca = JsonConvert.DeserializeObject<VMDestinatario>(modelo);

                TbDestinatario finca_creada = await _destinatarioService.Crear(_mapper.Map<TbDestinatario>(vmFinca));
                vmFinca = _mapper.Map<VMDestinatario>(finca_creada);
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
        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            GenericResponse<VMDestinatario> gResponse = new GenericResponse<VMDestinatario>();
            try
            {
                VMDestinatario vmFinca = JsonConvert.DeserializeObject<VMDestinatario>(modelo);

                TbDestinatario finca_editada = await _destinatarioService.Editar(_mapper.Map<TbDestinatario>(vmFinca));

                vmFinca = _mapper.Map<VMDestinatario>(finca_editada);
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

        [HttpPost]
        public async Task<IActionResult> Eliminar(int idDestinatario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _destinatarioService.Eliminar(idDestinatario);
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
