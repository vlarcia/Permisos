using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Net;
using Entity.DTOs;
using Microsoft.AspNetCore.Authorization;
using Permisos.WebApp.Utilidades.Response;
using Permisos.WebApp.Models.ViewModels;

namespace Permisos.WebApp.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashboardService;

        public DashBoardController(IDashBoardService dashboardService)
        {
            _dashboardService = dashboardService;
            
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();
            try
            {
                VMDashBoard vmDashBoard = new VMDashBoard();

                vmDashBoard.TotalPermisos = await _dashboardService.TotalPermisos();
                vmDashBoard.TotalDestinatarios = await _dashboardService.TotalDestinatarios();
                vmDashBoard.AlertasUltimoMes = await _dashboardService.AlertasMes();
                vmDashBoard.VencimientosMes = await _dashboardService.TotalVencimientoMes();
                vmDashBoard.TotalPermisosVencidosNoTramite = await _dashboardService.TotalPermisosVencidosNoTramite();



                List<VMListaDashBoard> listaRenovaciones = new List<VMListaDashBoard>();
                List<VMListaDashBoard> listaVencimientos = new List<VMListaDashBoard>();

              

                foreach (KeyValuePair<string, int> item in await _dashboardService.RenovacionesMes())
                {
                    listaRenovaciones.Add(new VMListaDashBoard()

                    {
                        Llave = item.Key,
                        Valor = item.Value
                    });
                }
                foreach (KeyValuePair<string, int> item in await _dashboardService.VencimientosMes())
                {
                    listaVencimientos.Add(new VMListaDashBoard()

                    {
                        Llave = item.Key,
                        Valor = item.Value
                    });
                }
                vmDashBoard.Renovaciones = listaRenovaciones;
                vmDashBoard.Vencimientos=listaVencimientos;
                

                gResponse.Estado=true;
                gResponse.Objeto=vmDashBoard;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;

            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerPermisosPorFecha(string fecha)
        {
            var gResponse = new GenericResponse<List<VMPermiso>>();

            try
            {
                if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime fechaParsed))
                {
                    gResponse.Estado = false;
                    gResponse.Mensaje = "Fecha inválida. Formato esperado: yyyy-MM-dd";
                    return BadRequest(gResponse);
                }

                var lista = await _dashboardService.ObtenerPermisosPorFecha(fechaParsed);

                gResponse.Estado = true;
                gResponse.Objeto = lista.Select(p => new VMPermiso
                {
                    IdPermiso = p.IdPermiso,
                    Nombre = p.Nombre,
                    FechaVencimiento = p.FechaVencimiento,
                    EstadoPermiso = p.EstadoPermiso,
                    Institucion = p.Institucion
                }).ToList();
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
