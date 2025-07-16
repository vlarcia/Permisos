using AutoMapper;
using Business.Implementacion;
using Business.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Permisos.WebApp.Models.ViewModels;
using Permisos.WebApp.Utilidades.Response;

namespace Permisos.WebApp.Controllers
{
    public class AreaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAreaService _areaService;
        public AreaController(IAreaService areaService, IMapper mapper)
        {
            _areaService = areaService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaAreas()
        {
            List<VMArea> vmAreaLista = _mapper.Map<List<VMArea>>(await _areaService.Lista());
            return StatusCode(StatusCodes.Status200OK, vmAreaLista);

        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {            
            var areas = await _areaService.Lista();
            var vmAreas = _mapper.Map<List<VMArea>>(areas);

            return StatusCode(StatusCodes.Status200OK, new { data = vmAreas });


        }
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMArea> gResponse = new GenericResponse<VMArea>();
            try

            {
                VMArea vmArea = JsonConvert.DeserializeObject<VMArea>(modelo);

                TbArea finca_creada = await _areaService.Crear(_mapper.Map<TbArea>(vmArea));
                vmArea = _mapper.Map<VMArea>(finca_creada);
                gResponse.Estado = true;
                gResponse.Objeto = vmArea;

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
            GenericResponse<VMArea> gResponse = new GenericResponse<VMArea>();
            try
            {
                VMArea vmArea = JsonConvert.DeserializeObject<VMArea>(modelo);

                TbArea finca_editada = await _areaService.Editar(_mapper.Map<TbArea>(vmArea));

                vmArea = _mapper.Map<VMArea>(finca_editada);
                gResponse.Estado = true;
                gResponse.Objeto = vmArea;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int idArea)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _areaService.Eliminar(idArea);
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


