using Microsoft.AspNetCore.Mvc;
using Encadenamiento.WebApp.Models.ViewModels;
using Business.Interfaces;
using Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;



namespace Encadenamiento.WebApp.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public AccesoController(IUsuarioService usuarioService)
        {
                _usuarioService = usuarioService;

        }
        public IActionResult Login()
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            if (claimuser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult RestablecerClave()
        {
       
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Login(VMUsuarioLogin modelo)
        {
            
            Usuario usuaro_encontrado = await _usuarioService.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);
            if (usuaro_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }
            ViewData["Mensaje"]=null;
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuaro_encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuaro_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuaro_encontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuaro_encontrado.UrlFoto),               
            }; 
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties() {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesion
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(VMUsuarioLogin modelo)
        {
            try
            {
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";
                bool resultado=await _usuarioService.RestablecerClave(modelo.Correo, urlPlantillaCorreo);
                if (resultado)
                {
                    ViewData["Mensaje"] = "Listo su contraseña fue restablecida.  Revise su correo";
                    ViewData["MensajeError"] = null;
                }
                else
                {
                    ViewData["Mensaje"] = null;
                    ViewData["MensajeError"] = "Hubo un problema.  Por favor intente de nueva más tarde...";
                }
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = null;
                ViewData["MensajeError"] = ex.Message;
            }

            return View();
        }
    }
}
