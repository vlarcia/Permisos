using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Permisos.WebApp.Controllers
{
    public class ReportePermisosController : Controller
    {
        private readonly IPermisoService _permisoService;

        public ReportePermisosController(IPermisoService permisoService)
        {
            _permisoService = permisoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            var lista = await _permisoService.Lista();

            var hoy = DateTime.Now.Date;
            var tresMeses = hoy.AddMonths(3);

            var resumen = new
            {
                total = lista.Count(),
                porEstado = lista
                    .GroupBy(p => p.EstadoPermiso)
                    .Select(g => new { estado = g.Key, total = g.Count() }),
                porCriticidad = lista
                    .GroupBy(p => p.Criticidad)
                    .Select(g => new { criticidad = g.Key, total = g.Count() }),
                vencenPronto = lista
                    .Where(p => p.FechaVencimiento >= hoy && p.FechaVencimiento <= tresMeses)
                    .OrderBy(p => p.FechaVencimiento)
                    .Select(p => new
                    {
                        idPermiso = p.IdPermiso,
                        nombre = p.Nombre,
                        fechaVencimiento = p.FechaVencimiento,
                        encargado = p.Encargado,
                        estadoPermiso = p.EstadoPermiso,
                        criticidad = p.Criticidad
                    }).ToList()
            };

            return Json(new { estado = true, data = resumen });
        }


    }
}
