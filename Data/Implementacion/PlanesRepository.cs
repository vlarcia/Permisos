using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data.DBContext;
using Data.Interfaces;
using Entity;


namespace Data.Implementacion
{
    public class PlanesRepository : GenericRepository<PlanesTrabajo>, IPlanesRepository
    {
        private readonly CumplimientoContext _dbContext;
        public PlanesRepository(CumplimientoContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PlanesTrabajo> Registrar(PlanesTrabajo entidad)
        {
            PlanesTrabajo planGenerado = new PlanesTrabajo();

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // 1. Guardar el plan de trabajo con el modelo de Actividades 
                    _dbContext.PlanesTrabajos.Add(entidad);
                    await _dbContext.SaveChangesAsync();                    
                    planGenerado = entidad;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return planGenerado;  // Retornar el plan con todas las actividades asociadas
        }

        public async Task<List<Actividad>> Reporte(DateTime Fechainicio, DateTime Fechafin)
        {
            List<Actividad>listaResumen = await _dbContext.Actividades

                .Include(a => a.IdActividad)
                .Include(a => a.Descripcion)
                .Include(f => f.IdFincaNavigation.Descripcion)
                .Include(p => p.IdPlanNavigation.Descripcion)                                
                .Where(act=> act.IdPlanNavigation.FechaIni.Value.Date >= Fechainicio &&
                             act.IdPlanNavigation.FechaFin.Value.Date <= Fechafin).ToListAsync();

            return listaResumen;
        }
     
    }
}
