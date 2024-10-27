using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Encadenamiento.WebApp.Models.ViewModels;
using Encadenamiento.WebApp.Utilidades.Response;
using Business.Interfaces;
using Entity;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class CheckListController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICheckListService _checklistService;
        
        public CheckListController(ICheckListService checklistService,
          
           IMapper mapper)
        {
            _checklistService = checklistService;
        
            _mapper = mapper;

        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMCheckList> vmChecklistLista = _mapper.Map<List<VMCheckList>>(await _checklistService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmChecklistLista });
        }
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            GenericResponse<VMCheckList> gResponse = new GenericResponse<VMCheckList>();
            try

            {
                VMCheckList vmChecklist = JsonConvert.DeserializeObject<VMCheckList>(modelo);
                             
                CheckList req_creada = await _checklistService.Crear(_mapper.Map<CheckList>(vmChecklist));
                vmChecklist = _mapper.Map<VMCheckList>(req_creada);
                gResponse.Estado=true;
                gResponse.Objeto = vmChecklist;

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
            GenericResponse<VMCheckList> gResponse = new GenericResponse<VMCheckList>();
            try
            {
                VMCheckList vmChecklist = JsonConvert.DeserializeObject<VMCheckList>(modelo);                
              
                CheckList req_editada = await _checklistService.Editar(_mapper.Map<CheckList>(vmChecklist));

                vmChecklist = _mapper.Map<VMCheckList>(req_editada);
                gResponse.Estado = true;
                gResponse.Objeto = vmChecklist;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idRequisito)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _checklistService.Eliminar(idRequisito);
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
