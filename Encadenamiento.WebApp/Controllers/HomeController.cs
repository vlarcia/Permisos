using Microsoft.AspNetCore.Mvc;
using Encadenamiento.WebApp.Models;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Encadenamiento.WebApp.Models.ViewModels;
using Encadenamiento.WebApp.Utilidades.Response;
using Business.Interfaces;
using Entity;

namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public HomeController(IUsuarioService usuarioServce, IMapper mapper)
        {
            _usuarioService = usuarioServce;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Perfil()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario=claimUser.Claims
                    .Where(c=> c.Type==ClaimTypes.NameIdentifier)
                    .Select(c=>c.Value).SingleOrDefault();

                VMUsuario usuario = _mapper.Map<VMUsuario>(await _usuarioService.ObtenerPorId(int.Parse(idUsuario)));
                response.Estado = true;
                response.Objeto = usuario;
            }
            catch (Exception ex) {
                response.Estado=false;
                response.Mensaje=ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK,response);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMUsuario modelo)
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                Usuario entidad = _mapper.Map<Usuario>(modelo);
                entidad.IdUsuario=int.Parse(idUsuario);
                bool resultado = await _usuarioService.GuardarPerfil(entidad);
                
                response.Estado = resultado;                
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }


        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<VMUsuario> response = new GenericResponse<VMUsuario>();
            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                bool resultado = await _usuarioService.CambiarClave(
                    int.Parse(idUsuario),
                    modelo.claveActual,
                    modelo.claveNueva
                    );

                response.Estado = resultado;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}