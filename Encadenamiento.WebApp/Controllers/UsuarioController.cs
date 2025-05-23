using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Encadenamiento.WebApp.Models.ViewModels;
using Encadenamiento.WebApp.Utilidades.Response;
using Business.Interfaces;
using Entity;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Authorization;

namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        public UsuarioController(IUsuarioService usuarioService,
           IRolService rolservice,
           IMapper mapper)
        {
            _usuarioService = usuarioService;
            _rolService = rolservice;
            _mapper = mapper;

        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> listaRoles()
        {
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolService.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);

        }
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(await _usuarioService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista });
        }
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try

            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string nombreFoto = "";
                Stream fotoStream = null;
                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);                    
                    nombreFoto = string.Concat(vmUsuario.IdUsuario.ToString(), nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";                
                Usuario usuario_creado = await _usuarioService.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);
                vmUsuario = _mapper.Map<VMUsuario>(usuario_creado);
                gResponse.Estado=true;
                gResponse.Objeto = vmUsuario;

            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string nombreFoto = "";
                Stream fotoStream = null;
                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(vmUsuario.IdUsuario.ToString(), nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }             
                Usuario usuario_editado = await _usuarioService.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);
                vmUsuario = _mapper.Map<VMUsuario>(usuario_editado);
                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _usuarioService.Eliminar(idUsuario);
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
