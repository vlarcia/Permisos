using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Permisos.WebApp.Controllers
{
    public class ReporteAlertasController : Controller
    {
        private readonly IAlertaService _alertaService;

        public ReporteAlertasController(IAlertaService alertaService)
        {
            _alertaService = alertaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            var lista = await _alertaService.Lista();

            var resumen = new
            {
                Total = lista.Count(),
                PorResultado = lista.GroupBy(a => a.Resultado)
                                    .Select(g => new { Resultado = g.Key, Total = g.Count() }),
                PorMedio = lista.GroupBy(a => a.MedioEnvio)
                                .Select(g => new { Medio = g.Key, Total = g.Count() }),
                UltimasAlertas = lista.OrderByDescending(a => a.FechaEnvio)
                                      .Take(30)
                                      .Select(a => new
                                      {
                                          a.IdAlerta,
                                          a.IdPermiso,
                                          a.Mensaje,
                                          a.Resultado,
                                          a.MedioEnvio,
                                          FechaEnvio = a.FechaEnvio.ToString("yyyy-MM-dd HH:mm:ss")
                                      })
            };

            return Json(new { estado = true, data = resumen });
        }
    }
}
