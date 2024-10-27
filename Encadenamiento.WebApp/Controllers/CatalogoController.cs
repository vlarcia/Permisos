using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Encadenamiento.WebApp.Controllers
{
    [Authorize]
    public class CatalogoController : Controller
    {
        public IActionResult Empleado()
        {
            return View();
        }
        public IActionResult Insumo()
        {
            return View();
        }
        public IActionResult Unidades()
        {
            return View();
        }
        public IActionResult Labores()
        {
            return View();
        }
        public IActionResult Centrocosto()
        {
            return View();
        }
        public IActionResult Proveedor()
        {
            return View();
        }
        public IActionResult Producto()
        {
            return View();
        }
        public IActionResult Categoria()
        {
            return View();
        }

    }
}
