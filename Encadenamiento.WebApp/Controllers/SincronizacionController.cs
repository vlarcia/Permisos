using AutoMapper;
using Business.Implementacion;
using Business.Interfaces;
using Encadenamiento.WebApp.Models.ViewModels;
using Encadenamiento.WebApp.Utilidades.Response;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Encadenamiento.WebApp.Controllers
{
    public class SincronizacionController : Controller
    {        
        private readonly ISincronizaService _sincronizaService;        
        private readonly IMapper _mapper;   
        public SincronizacionController(ISincronizaService sincronizaService, IMapper mapper)
        {
            _sincronizaService = sincronizaService;
            _mapper = mapper;
        }        
        public IActionResult SincronizaVisita()
        {
            return View();
        }
        public IActionResult AplicaVisita()
        {
            return View();
        }
        public IActionResult SincronizaMoviles()
        {
            return View();
        }
        public IActionResult Android()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaAndroid()
        {
            List<AndroidId> AndroidLista = await _sincronizaService.ListaAndroid();
            return StatusCode(StatusCodes.Status200OK, new { data = AndroidLista });
        }

        [HttpGet]
        public async Task<IActionResult> ListaVisita()   //Lista las visitas que no han sido sincronizadas
        {
            List<VMVisita> vmVisitaLista = _mapper.Map<List<VMVisita>>(await _sincronizaService.ListaVisita());
            return StatusCode(StatusCodes.Status200OK, new { data = vmVisitaLista });
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modeloAndroid)
        {
            GenericResponse<AndroidId> gResponse = new GenericResponse<AndroidId>();
            try
            {
                AndroidId detalleAndroid = JsonConvert.DeserializeObject<AndroidId>(modeloAndroid);                
                AndroidId android_creado = await _sincronizaService.Crear(detalleAndroid);                    
                gResponse.Estado = true;
                gResponse.Objeto = android_creado;                
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] string modeloAndroid)
        {
            GenericResponse<AndroidId> gResponse = new GenericResponse<AndroidId>();
            try
            {
                AndroidId detalleAndroid = JsonConvert.DeserializeObject<AndroidId>(modeloAndroid);          
                AndroidId android_editado = await _sincronizaService.Editar(detalleAndroid);
                gResponse.Estado = true;
                gResponse.Objeto = android_editado;                
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
        [HttpPost]
        public async Task<IActionResult> AplicarVisita([FromForm] string modelovisita) 
        {
            GenericResponse<VMVisita> gResponse = new GenericResponse<VMVisita>();
            try
            {   
                Visita visita_aplica = JsonConvert.DeserializeObject<Visita>(modelovisita);          
                
                Visita  visita_aplicada  = await _sincronizaService.AplicaVisita(visita_aplica);                
                VMVisita vmVisita = _mapper.Map<VMVisita>(visita_aplicada);
                gResponse.Estado=true;
                gResponse.Objeto = vmVisita;                     
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int idAndroid)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _sincronizaService.Eliminar(idAndroid);
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
