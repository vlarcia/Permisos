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
    public class VisitaRepository : GenericRepository<Visita>, IVisitaRepository
    {
        private readonly CumplimientoContext _dbContext;
        public VisitaRepository(CumplimientoContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Visita> Registrar(Visita entidad)
        {
            Visita visitaGenerada= new Visita();

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // 1. Guardar la visita con el modelo de Actividades 
                    _dbContext.Visitas.Add(entidad);
                    await _dbContext.SaveChangesAsync();                    
                    visitaGenerada = entidad;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return visitaGenerada;  // Retornar el plan con todas las actividades asociadas
        }

        public async Task<List<DetalleVisita>> Reporte(DateTime Fechainicio, DateTime Fechafin)
        {
            List<DetalleVisita>listaResumen = await _dbContext.DetalleVisitas

                .Include(a => a.Avances)
                .Include(ac => ac.IdActividadNavigation.Descripcion)                                
                .Where(act=> act.Fecha.Value.Date >= Fechainicio &&
                             act.Fecha.Value.Date <= Fechafin).ToListAsync();

            return listaResumen;
        }
     
    }
}
