using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.DBContext;
using Data.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;



namespace Business.Implementacion
{
    public class RevisionService : IRevisionService
    {
        private readonly IGenericRepository<Revisione> _repositorioRev;
        private readonly IGenericRepository<CheckList> _repositorioReq;
        private readonly CumplimientoContext _dbcontext;
        public RevisionService(IGenericRepository<Revisione> repositorioRev, IGenericRepository<CheckList> repositorioReq,
                                CumplimientoContext context)
        {
            _repositorioRev = repositorioRev;
            _repositorioReq = repositorioReq;   
            _dbcontext = context;   
        }

        public async Task<List<CheckList>> ListaRequisitos()
        {
            IQueryable<CheckList> query = await _repositorioReq.Consultar();
            return query.ToList();
        }
        public async Task<List<Revisione>> Lista()
        {
            IQueryable<Revisione> query = await _repositorioRev.Consultar();

            // Agrupar por IdFinca y Fecha, y tomar el primer registro de cada grupo
            var resultado = query
                .Include(f => f.IdFincaNavigation)
                .Include(r => r.IdRequisitoNavigation)
                .GroupBy(r => new { r.IdFinca, r.Fecha })  // Agrupar por Finca y Fecha                
                .Select(g => g.First())                    // Tomar el primer registro de cada grupo
    
                .ToList();

            return resultado;
        }

        public async Task<Revisione> Crear(List<Revisione> entidad)
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
                {
                
                Revisione revision_existe = await _repositorioRev.Obtener(r => r.IdFinca == entidad.First().IdFinca && r.Fecha == entidad.First().Fecha);
                if (revision_existe != null)
                    throw new TaskCanceledException("Ya existe una revisión para la finca "+revision_existe.IdFinca.ToString() +
                                                    " y con fecha "+revision_existe.Fecha.ToString());

                Revisione revision_creada = new Revisione();
                try
                {                    
                    foreach (var revision in entidad)
                    {
                         // Validar cada revisión o aplicar lógica adicional si es necesario
                        if (revision.IdFinca.Value!=null && revision.Fecha.Value!=null)
                        {
                            _dbcontext.Revisiones.Add(revision);
                            revision_creada = revision;
                        }
                        else
                        {
                            throw new TaskCanceledException($"No se pudo crear la Revision {revision.IdFincaNavigation.CodFinca}!");                            
                        }
                    }
                    // Guardar cambios en la base de datos
                    await _dbcontext.SaveChangesAsync();

                    // Confirmar la transacción
                     await transaction.CommitAsync();
                    if (revision_creada != null)
                    {
                        await _dbcontext.Entry(revision_creada)
                            .Reference(r => r.IdFincaNavigation).LoadAsync();
                        await _dbcontext.Entry(revision_creada)
                            .Reference(r => r.IdRequisitoNavigation).LoadAsync();
                    }
                    return revision_creada;
                }
                    catch (Exception ex)
                    {
                        // Si ocurre un error, se deshacen los cambios
                        await transaction.RollbackAsync();
                        throw ex;
                    }                
                }
            }

        public async Task<Revisione> Editar(List<Revisione> entidad)
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
            {
                Revisione revision_modificada = null;
                try
                {
                    foreach (var revision in entidad)
                    {
                        // Busca la entidad existente en el contexto
                         revision_modificada = await _dbcontext.Revisiones
                            .FirstOrDefaultAsync(r => r.IdRevision == revision.IdRevision);

                        // Validar si la revisión existe
                        if (revision_modificada != null)
                        {
                            // Mapea los valores de la nueva entidad a la existente
                            _dbcontext.Entry(revision_modificada).CurrentValues.SetValues(revision);
                        }
                        else
                        {
                            throw new TaskCanceledException($"No se pudo modificar la revisión de la finca {revision.IdFincaNavigation.CodFinca}!");
                        }
                    }

                    // Guardar cambios en la base de datos
                    await _dbcontext.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();
                    if (revision_modificada != null)
                    {
                        await _dbcontext.Entry(revision_modificada)
                            .Reference(r => r.IdFincaNavigation).LoadAsync();
                        await _dbcontext.Entry(revision_modificada)
                            .Reference(r => r.IdRequisitoNavigation).LoadAsync();
                    }
                    return revision_modificada;
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, se deshacen los cambios
                    await transaction.RollbackAsync();
                    throw ex;
                }
         
            }
        }


        public async Task<bool> Eliminar(List<int> entidad)
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
            {
                Revisione revision_eliminada = new Revisione();
                try
                {
                    foreach (var idrevision in entidad)
                    {
                        revision_eliminada = await _repositorioRev.Obtener(r => r.IdRevision == idrevision);

                        // Validar cada revisión que exista para eliminar
                        if (revision_eliminada != null)
                        {
                            _dbcontext.Revisiones.Remove(revision_eliminada);
                        }
                        else
                        {
                            throw new TaskCanceledException($"No se pudo eliminar la Revision de la finca {revision_eliminada.IdFincaNavigation.CodFinca}!");
                        }
                    }
                    // Guardar cambios en la base de datos
                    await _dbcontext.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, se deshacen los cambios
                    await transaction.RollbackAsync();
                    throw ex;
                }
                return true;
            }

        }    
        public async Task<List<Revisione>> ObtenerRevision(int idfinca, string fecha)
        {
             // Convertir el string de la fecha a un objeto DateTime
             DateTime lafecha = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
             IQueryable<Revisione> query = await _repositorioRev.Consultar();

            // Filtrar las revisiones por finca y fecha
            var resultado = query
                .Include(f => f.IdFincaNavigation)
                .Include(r => r.IdRequisitoNavigation)
                .Where(r => r.IdFinca == idfinca && r.Fecha == lafecha) // Comparar por IdFinca y la fecha                
                .ToList();

            return resultado;
        }


    }
}
