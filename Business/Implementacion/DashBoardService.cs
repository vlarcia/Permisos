using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity;
using Entity.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Data.Implementacion;


namespace Business.Implementacion
{
    public class DashBoardService : IDashBoardService

    {
        private readonly IPlanesRepository _planesRepositorio;
        private readonly IGenericRepository<Actividad> _actividadRepositorio;
        private readonly IGenericRepository<Visita> _visitaRepositorio;
        private readonly IGenericRepository<Revisione> _revisionRepositorio;
        private DateTime FechaInicial=DateTime.Now;

        public DashBoardService(IPlanesRepository planesRepositorio,
                                IGenericRepository<Actividad> actividadRepositorio,
                                IGenericRepository<Visita> visitaRepositorio,
                                IGenericRepository<Revisione> revisionRepositorio)
        { 
            _planesRepositorio=planesRepositorio;
            _actividadRepositorio=actividadRepositorio;
            _visitaRepositorio=visitaRepositorio;
            _revisionRepositorio=revisionRepositorio;
            FechaInicial = FechaInicial.AddDays(-30);
        


        }
        public async Task<string> TotalFincas()
        {
            try
            {
                IQueryable<Revisione> query = await _revisionRepositorio.Consultar();
                int total = query.Select(r => r.IdFinca).Distinct().Count();                
                return Convert.ToString(total, new CultureInfo("es-NI"));
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> TotalPlanesActivos()
        {
            try {
                IQueryable<PlanesTrabajo> query = await _planesRepositorio.Consultar(p => p.Estado.Trim() == "EN PROCESO" && p.FechaIni>= FechaInicial.Date);
                int total = query.Count();
                return Convert.ToString(total, new CultureInfo("es-NI"));
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> TotalActividadesActivas()
        {
            try
            {
                IQueryable<Actividad> query = await _actividadRepositorio.Consultar(a => a.Estado.Trim() == "EN PROCESO" && a.FechaIni >= FechaInicial.Date);
                int total = query.Count();
                return Convert.ToString(total, new CultureInfo("es-NI"));
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> RevisionesUltimoMes()
        {
            try
            {
                IQueryable<Revisione> query = await _revisionRepositorio.Consultar(r => r.Fecha >= FechaInicial.Date);
                int total = query.Select(r => new { r.IdFinca, r.Fecha }).Distinct().Count();                
                return Convert.ToString(total, new CultureInfo("es-NI"));
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> VisitasUltimoMes()
        {
            try
            {
                IQueryable<Visita> query = await _visitaRepositorio.Consultar(v => v.Fecha >= FechaInicial.Date);
                int total = query.Count();
                return Convert.ToString(total, new CultureInfo("es-NI"));
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> ActividadesCompletadasUltimoMes()
        {            
            try
            {
                IQueryable<Actividad> query = await _actividadRepositorio
                          .Consultar(a => a.FechaUltimarevision >= FechaInicial.Date && a.Estado == "FINALIZADA");
                Dictionary<string, int> resultado = query
                    .GroupBy(a=> a.FechaUltimarevision.Value.Date).OrderByDescending(g=> g.Key)
                    .Select(da=> new {fecha=da.Key.ToString("dd/MM/yyyy"), total=da.Count()})
                    .ToDictionary(keySelector: r=> r.fecha, elementSelector: r => r.total);  
                    ;
                return resultado;            
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> FincasVisitadasUltimoMes()
        {
            try
            {
                IQueryable<Visita> query = await _visitaRepositorio
                          .Consultar(v => v.Fecha >= FechaInicial.Date);

                Dictionary<string, int> resultado = query
                    .GroupBy(a => a.Fecha.Value.Date).OrderByDescending(g => g.Key)
                    .Select(da => new { fecha = da.Key.ToString("dd/MM/yyyy"), total = da.Count() })
                    .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);
                ;
                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<CumplimientoDTO>> ObtenerFincasConCumplimiento()
        {
            try
            {
                IQueryable<Revisione> query = await _revisionRepositorio.Consultar();


                // Incluir IdFincaNavigation
                var revisionesConFinca = query.Include(r => r.IdFincaNavigation).AsEnumerable();

                var fincasConCumplimiento = revisionesConFinca
                    .GroupBy(r => r.IdFinca)
                    .Select(grupo => new
                    {
                        IdFinca = grupo.Key,
                        RevisionReciente = grupo.OrderByDescending(r => r.Fecha).FirstOrDefault()
                    })
                    .Where(g => g.RevisionReciente != null && g.RevisionReciente.IdFincaNavigation != null)
                    .Select(g => new CumplimientoDTO
                    {
                        CodFinca = g.RevisionReciente.IdFincaNavigation.CodFinca,
                        NombreFinca = g.RevisionReciente.IdFincaNavigation.Descripcion,
                        Cumplimiento = g.RevisionReciente.Cumplimiento,
                        FechaUltimarevision = g.RevisionReciente.Fecha
                    })
                    .ToList();

                return fincasConCumplimiento;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener cumplimiento de fincas: {ex.Message}", ex);
            }
        }


        //public async Task<List<CumplimientoDTO>> ObtenerFincasConCumplimiento()
        //{            
        //    try
        //    {
        //        IQueryable<Revisione> query = await _revisionRepositorio.Consultar();

        //        var fincasConCumplimiento = query
        //            .AsEnumerable() // Traemos los datos a memoria para operar en la aplicación
        //            .GroupBy(r => r.IdFinca)
        //            .Select(grupo => new
        //            {
        //                IdFinca = grupo.Key,
        //                RevisionReciente = grupo.OrderByDescending(r => r.Fecha).FirstOrDefault()
        //            })
        //            .Select(g => new CumplimientoDTO
        //            {
        //                CodFinca = g.RevisionReciente.IdFincaNavigation.CodFinca,
        //                NombreFinca = g.RevisionReciente.IdFincaNavigation.Descripcion,
        //                Cumplimiento = g.RevisionReciente.Cumplimiento,
        //                FechaUltimarevision = g.RevisionReciente.Fecha
        //            })
        //            .ToList();

        //        return fincasConCumplimiento;

        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

    }
}
