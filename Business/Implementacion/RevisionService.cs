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
        private readonly IGenericRepository<Revision> _repositorioRev2;
        private readonly IGenericRepository<CheckList> _repositorioReq;
        private readonly IFireBaseService _firebaseService;
        private readonly CumplimientoContext _dbcontext;
        public RevisionService(IGenericRepository<Revisione> repositorioRev, IGenericRepository<Revision> repositorioRev2,
                               IGenericRepository<CheckList> repositorioReq, IFireBaseService firebaseService,  CumplimientoContext context)
        {
            _repositorioRev = repositorioRev;
            _repositorioRev2 = repositorioRev2;
            _repositorioReq = repositorioReq;
            _dbcontext = context;
            _firebaseService = firebaseService; 
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
                    throw new TaskCanceledException("Ya existe una revisión para la finca " + revision_existe.IdFinca.ToString() +
                                                    " y con fecha " + revision_existe.Fecha.ToString());

                Revisione revision_creada = new Revisione();                
                try
                {
                    foreach (var revision in entidad)
                    {
                        // Validar cada revisión o aplicar lógica adicional si es necesario
                        if (revision.IdFinca != null && revision.Fecha != null)
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

        public async Task<bool> Eliminar(List<int> entidad, int entidad2)
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
            {
                string nombrefoto1;
                string nombrefoto2;
                Revisione revision_eliminada = new Revisione();
                Revision fotosrevision_eliminada = new Revision();
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
                            throw new TaskCanceledException($"No se pudo eliminar la Revision con número de detalle {idrevision}!");
                        }
                    }

                    fotosrevision_eliminada = await _repositorioRev2.Obtener(r => r.IdReg == entidad2);
                    if (fotosrevision_eliminada != null)
                    {
                        nombrefoto1 = fotosrevision_eliminada.Nombrefoto1;   //Para borrar las fotos de la visita
                        nombrefoto2 = fotosrevision_eliminada.Nombrefoto2;
                        _dbcontext.Revisions.Remove(fotosrevision_eliminada);
                    }
                    else
                    {
                        throw new TaskCanceledException($"No se pudo eliminar la Revision con registro general {entidad2}!");
                    }
                    // Guardar cambios en la base de datos
                    await _dbcontext.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();

                    await _firebaseService.EliminarStorage("carpeta_revision", nombrefoto1);
                    await _firebaseService.EliminarStorage("carpeta_revision", nombrefoto2);
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
        public async Task<List<Revisione>> ObtenerRevision(int idfinca, string fecha, int grupo)
        {
            try
            {
                IQueryable<Revisione> query = await _repositorioRev.Consultar(); // No ejecutar la consulta inmediatamente

                // Incluir las relaciones de navegación
                query = query
                    .Include(f => f.IdFincaNavigation)
                    .Include(r => r.IdRequisitoNavigation);

                // Filtrar por fecha si se proporciona
                if (!string.IsNullOrEmpty(fecha))
                {
                    DateTime lafecha = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    query = query.Where(r => r.IdFinca == idfinca && r.Fecha == lafecha); // Filtro por finca y fecha
                }
                // Filtrar solo por finca si se proporciona
                else if (idfinca != 0)
                {
                    query = query.Where(r => r.IdFinca == idfinca); // Filtrar por finca
                }
                // Filtrar por grupo si se proporciona
                else if (grupo != 0)
                {                  
                    var temporal =query.ToList(); // Filtrar por grupo                    
                    temporal=temporal.Where(r => r.IdFincaNavigation.Grupo == grupo).ToList();
                    return temporal;
                }

                // Ejecutar la consulta y obtener los resultados
                var resultado = await query.ToListAsync(); // Ahora ejecutamos la consulta cuando se aplican todos los filtros

                return resultado;
            }
            catch (Exception ex)
            {
                throw ex; // Capturar el error, pero sería bueno manejarlo más adecuadamente (ej. log)
            }
        }
        public async Task<List<Revision>> ListaRevisions(int envio)
        {
            IQueryable<Revision> query = await _repositorioRev2.Consultar();
            List<Revision>  resultado= null;
            if (envio != 1)
            {
                 resultado = query
                    .Include(f => f.IdFincaNavigation)
                    .ToList();
            }
            else
            {
                resultado = query
                    .Include(f => f.IdFincaNavigation)
                    .Where(s=>s.SentTo == 0)
                    .ToList();
            }

            return resultado;
        }

        public async Task<Revision> CrearRevisions(Revision entidad, Stream foto1 = null, string NombreFoto1 = "", Stream foto2 = null, string NombreFoto2 = "")
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
            {

                Revision fotosrevision_existe = await _repositorioRev2.Obtener(r => r.IdFinca == entidad.IdFinca && r.Fecha == entidad.Fecha);
                if (fotosrevision_existe != null)
                    throw new TaskCanceledException("Ya existe un registro GENERAL de la revisión para la finca " + fotosrevision_existe.IdFinca.ToString() +
                                                    " y con fecha " + fotosrevision_existe.Fecha.ToString());

                Revision fotosrevision_creada = new Revision();
                try
                {
                    // Validar cada revisión o aplicar lógica adicional si es necesario
                    if (entidad.IdFinca != null && entidad.Fecha != null)
                    {
                        if (foto1 != null)
                        {
                            string UrlFoto1 = await _firebaseService.SubirStorage(foto1, "carpeta_revision", NombreFoto1);
                            entidad.Urlfoto1 = UrlFoto1;
                        }
                        if (foto2 != null)
                        {
                            string UrlFoto2 = await _firebaseService.SubirStorage(foto2, "carpeta_revision", NombreFoto2);
                            entidad.Urlfoto2 = UrlFoto2;
                        }


                        _dbcontext.Revisions.Add(entidad);
                        fotosrevision_creada = entidad;
                    }
                    else
                    {
                        throw new TaskCanceledException($"No se pudo crear registro GENERAL de Revision para finca {entidad.IdFinca}!");
                    }
                    

                    // Guardar cambios en la base de datos
                    await _dbcontext.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();
                    if (fotosrevision_creada != null)
                    {
                        await _dbcontext.Entry(fotosrevision_creada)
                            .Reference(r => r.IdFincaNavigation).LoadAsync();                        
                    }
                    //Agregamos la foto al repositorio luego de agregar el registro
               

                    return fotosrevision_creada;
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, se deshacen los cambios
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
        }

        public async Task<Revision> EditarRevisions(Revision entidad, Stream foto1 = null, string NombreFoto1 = "", Stream foto2 = null, string NombreFoto2 = "")
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
            {
                Revision fotosrevision_modificada = null;
                try
                {                    
                    // Busca la entidad existente en el contexto
                    fotosrevision_modificada = await _dbcontext.Revisions.FirstOrDefaultAsync(r => r.IdReg == entidad.IdReg);

                    // Validar si la revisión existe
                    if (fotosrevision_modificada != null)
                    {
                        entidad.Urlfoto1 = fotosrevision_modificada.Urlfoto1;
                        entidad.Nombrefoto1 = fotosrevision_modificada.Nombrefoto1;
                        entidad.Urlfoto2 = fotosrevision_modificada.Urlfoto2;
                        entidad.Nombrefoto2 = fotosrevision_modificada.Nombrefoto2;                        

                        if (foto1 != null && NombreFoto1 != fotosrevision_modificada.Nombrefoto1)
                        {
                            string UrlFoto1 = await _firebaseService.SubirStorage(foto1, "carpeta_revision", NombreFoto1);
                            entidad.Urlfoto1 = UrlFoto1;
                            entidad.Nombrefoto1 = NombreFoto1;
                        }
                        if (foto2 != null && NombreFoto2 != fotosrevision_modificada.Nombrefoto2)
                        {
                            string UrlFoto2 = await _firebaseService.SubirStorage(foto2, "carpeta_revision", NombreFoto2);
                            entidad.Urlfoto2 = UrlFoto2;
                            entidad.Nombrefoto2 = NombreFoto2;
                        }
                        // Mapea los valores de la nueva entidad a la existente
                        _dbcontext.Entry(fotosrevision_modificada).CurrentValues.SetValues(entidad);
                    }
                    else
                    {
                        throw new TaskCanceledException($"No se pudo modificar el registro GENERAL de la revisión de la finca {entidad.IdFinca}!");
                    }
                    

                    // Guardar cambios en la base de datos
                    await _dbcontext.SaveChangesAsync();

                    // Confirmar la transacción
                    await transaction.CommitAsync();                    
                    if (fotosrevision_modificada != null)
                    {
                        await _dbcontext.Entry(fotosrevision_modificada)
                            .Reference(r => r.IdFincaNavigation).LoadAsync();                        
                    }                    

                    return fotosrevision_modificada;
                }
                catch (Exception ex)
                {
                    // Si ocurre un error, se deshacen los cambios
                    await transaction.RollbackAsync();
                    throw ex;
                }

            }
        }
        public async Task<Revision> ObtenerRevisionGeneral(int idfinca, string fecha)
        {
            try
            {
                Revision resultado = null;
                if (!string.IsNullOrEmpty(fecha) && idfinca != 0)
                {
                   DateTime lafecha = DateTime.ParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                   resultado = await _repositorioRev2.Obtener(r => r.IdFinca == idfinca && r.Fecha == lafecha);
                    

                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw ex; // Capturar el error, pero sería bueno manejarlo más adecuadamente (ej. log)
            }
        }
    }
}
