using Microsoft.AspNetCore.Mvc;
using Encadenamiento.WebApp.Models.ViewModels;
using Encadenamiento.WebApp.Utilidades.Response;
using Business.Interfaces;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Net;
using Entity.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Encadenamiento.WebApp.Controllers
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
                vmDashBoard.TotalFincas = await _dashboardService.TotalFincas();
                vmDashBoard.TotalPlanes = await _dashboardService.TotalPlanesActivos();
                vmDashBoard.TotalActividades = await _dashboardService.TotalActividadesActivas();
                vmDashBoard.TotalRevisiones = await _dashboardService.RevisionesUltimoMes();
                vmDashBoard.TotalVisitas = await _dashboardService.VisitasUltimoMes();

                List<VMListaDashBoard> listaActividadesCompletadas = new List<VMListaDashBoard>();
                List<VMListaDashBoard> listaFincasVisitadas = new List<VMListaDashBoard>();

                List<CumplimientoDTO> fincasDTO = await _dashboardService.ObtenerFincasConCumplimiento();
                List<VMCumplimiento> listaFincasCumplimiento = fincasDTO.Select(dto => new VMCumplimiento

                {
                    CodFinca = dto.CodFinca,
                    NombreFinca = dto.NombreFinca,
                    Cumplimiento = dto.Cumplimiento,
                    FechaUltimarevision = dto.FechaUltimarevision
                }).ToList();


                foreach (KeyValuePair<string, int> item in await _dashboardService.ActividadesCompletadasUltimoMes())
                {
                    listaActividadesCompletadas.Add(new VMListaDashBoard()

                    {
                        Llave = item.Key,
                        Valor = item.Value
                    });
                }
                foreach (KeyValuePair<string, int> item in await _dashboardService.FincasVisitadasUltimoMes())
                {
                    listaFincasVisitadas.Add(new VMListaDashBoard()

                    {
                        Llave = item.Key,
                        Valor = item.Value
                    });
                }
                vmDashBoard.FincasVisitadas = listaFincasVisitadas;
                vmDashBoard.ActividadesCompletas=listaActividadesCompletadas;
                vmDashBoard.CumplimientoGlobal = listaFincasCumplimiento;

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
    }
}
