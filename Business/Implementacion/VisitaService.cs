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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Business.Implementacion
{
    public class VisitaService : IVisitaService
    {
        private readonly IVisitaRepository _repositorioVisita;
        private readonly IGenericRepository<DetalleVisita> _repositorioDetVis;
        private readonly CumplimientoContext _dbContext;
        private readonly IFireBaseService _firebaseService;
        
        public VisitaService(IVisitaRepository repositorioVisita, IGenericRepository<DetalleVisita> repositorioDetVis,
                            CumplimientoContext dbContext, IFireBaseService firebaseService)
            
        {
            _repositorioVisita = repositorioVisita;
            _repositorioDetVis = repositorioDetVis;               
            _dbContext = dbContext;
            _firebaseService = firebaseService;
        }

        public async Task<List<Visita>> Lista(int envio)
        {
            if (envio != 1)
            {
                IQueryable<Visita> query = await _repositorioVisita.Consultar();
                return query.Include(f => f.IdFincaNavigation)
                            .Include(p => p.IdPlanNavigation)
                            .Include(d => d.DetalleVisita)
                            .ThenInclude(a => a.IdActividadNavigation)
                            .ToList();
            } else
            {
                IQueryable<Visita> query = await _repositorioVisita.Consultar();
                return query.Include(f => f.IdFincaNavigation)
                            .Include(p => p.IdPlanNavigation)
                            .Include(d => d.DetalleVisita)
                            .ThenInclude(a => a.IdActividadNavigation)
                            .Where(s=> s.SentTo==0)
                            .ToList();
            }
            
        }
        public async Task<List<DetalleVisita>> ListaDetalle()
        {
            IQueryable<DetalleVisita> query = await _repositorioDetVis.Consultar();

            return query.Include(p => p.IdActividadNavigation)                        
                        .ToList();
        }
        public async Task<Visita> Registrar(Visita entidad, Stream foto1=null, string NombreFoto1="", Stream foto2=null, string NombreFoto2="")
        {
            try
            {
                Visita visita_existe = await _repositorioVisita.Obtener(v => v.IdPlan == entidad.IdPlan && v.Fecha == entidad.Fecha);
                if (visita_existe != null)
                    throw new TaskCanceledException("Ya existe una visita de revision del plan de trabajo  " + visita_existe.IdPlan.ToString() +
                                                    " y con fecha " + visita_existe.Fecha.ToString());
                //Agregamos la foto al repositorio luego de agregar el registro
                if (foto1 != null)
                {
                    string UrlFoto1 = await _firebaseService.SubirStorage(foto1, "carpeta_visita", NombreFoto1);
                    entidad.Urlfoto1 = UrlFoto1;
                }
                if (foto2 != null)
                {
                    string UrlFoto2 = await _firebaseService.SubirStorage(foto2, "carpeta_visita", NombreFoto2);
                    entidad.Urlfoto2 = UrlFoto2;
                }
                Visita visita_creada = await _repositorioVisita.Registrar(entidad);  //Registrar esta en su repositorio propio
                if (visita_creada.IdVisita == 0)
                    throw new TaskCanceledException("No se puedo crear la Visita!");

                IQueryable<Visita> query = await _repositorioVisita.Consultar(u => u.IdVisita == visita_creada.IdVisita);
                visita_creada = query.Include(f => f.IdFincaNavigation)
                                     .Include(p => p.IdPlanNavigation)
                                     .Include(d=> d.DetalleVisita)
                                     .ThenInclude(act=>act.IdActividadNavigation)
                                     .First();

                return visita_creada;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Visita> Editar(Visita entidad, Stream foto1 = null, string NombreFoto1 = "", Stream foto2 = null, string NombreFoto2 = "")
        {            
            try
            {                                
                IQueryable<Visita> queryVisita = await _repositorioVisita.Consultar(p => p.IdVisita == entidad.IdVisita);
                Visita visita_editar = queryVisita.First();

                if (queryVisita == null)
                    throw new TaskCanceledException("La Visita no existe en la base de datos, no puede ser editado!");

                if (foto1 != null && NombreFoto1 != visita_editar.Nombrefoto1)
                {
                    string UrlFoto1 = await _firebaseService.SubirStorage(foto1, "carpeta_visita", NombreFoto1);
                    visita_editar.Urlfoto1 = UrlFoto1;
                    visita_editar.Nombrefoto1 = NombreFoto1;                    
                }
                if (foto2 != null && NombreFoto2 != visita_editar.Nombrefoto2)
                {
                    string UrlFoto2 = await _firebaseService.SubirStorage(foto2, "carpeta_visita", NombreFoto2);
                    visita_editar.Urlfoto2 = UrlFoto2;
                    visita_editar.Nombrefoto2 = NombreFoto2;
                }
                
                visita_editar.Observaciones = entidad.Observaciones;
                visita_editar.IdFinca = entidad.IdFinca;
                visita_editar.IdPlan = entidad.IdPlan;
                visita_editar.Zafra = entidad.Zafra;
                visita_editar.AndroidId = entidad.AndroidId;
                visita_editar.Responsable = entidad.Responsable;
                visita_editar.Mandador = entidad.Mandador;
                visita_editar.Longitud = entidad.Longitud;
                visita_editar.Latitud = entidad.Latitud;
                visita_editar.Fecha = entidad.Fecha;
                visita_editar.Sincronizado= entidad.Sincronizado;
                visita_editar.Aplicado = entidad.Aplicado;                

                foreach (var detalle in entidad.DetalleVisita)
                {
                    IQueryable<DetalleVisita> queryActividad = await _repositorioDetVis.Consultar(p => p.IdReg == detalle.IdReg);
                    DetalleVisita detalleeditar = queryActividad.First();
                    detalleeditar.Fecha = detalle.Fecha;
                    detalleeditar.Avances = detalle.Avances;
                    detalleeditar.Avanceanterior = detalle.Avanceanterior;
                    detalleeditar.Estado = detalle.Estado;
                    detalleeditar.Estadoanterior = detalle.Estadoanterior;
                    detalleeditar.Comentarios = detalle.Comentarios;
                    detalleeditar.Observaciones = detalle.Observaciones;

                    bool respuesta2 = await _repositorioDetVis.Editar(detalleeditar);
                    if (!respuesta2)
                        throw new TaskCanceledException("Hubo problemas en el detalle de la Visita y no se pudo editar!");
                }
                
                bool respuesta = await _repositorioVisita.Editar(visita_editar);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar la Visita.   Revise!");
                Visita visita_fueeditada = queryVisita
                                     .Include(f => f.IdFincaNavigation)
                                     .Include(p => p.IdPlanNavigation)
                                     .Include(d => d.DetalleVisita)
                                     .ThenInclude(act => act.IdActividadNavigation)
                                     .First();
                return visita_fueeditada;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idVisita)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    IQueryable<DetalleVisita> hay_detalles = await _repositorioDetVis.Consultar(v => v.IdVisita == idVisita);

                    if (hay_detalles != null)
                    {
                        foreach (var eldetalle in hay_detalles)
                        {
                            _dbContext.DetalleVisitas.Remove(eldetalle);
                        }
                    }

                    Visita visita_encontrada = await _repositorioVisita.Obtener(u => u.IdVisita == idVisita);
       

                    if (visita_encontrada == null)
                        throw new TaskCanceledException("La Visita no existe en la base de datos!");

                    string nombreFoto1 = visita_encontrada.Nombrefoto1;   //Para borrar las fotos de la visita
                    string nombreFoto2 = visita_encontrada.Nombrefoto2;

                    _dbContext.Visitas.Remove(visita_encontrada);
                        
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    await _firebaseService.EliminarStorage("carpeta_visita", nombreFoto1);
                    await _firebaseService.EliminarStorage("carpeta_visita", nombreFoto2);
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync(); // Deshacer cambios si ocurre un error
                    throw;
                }
            }
        }


        public async Task<bool> EliminarDetalle(int IdReg)
        {
            try
            {
                DetalleVisita det_encontrado = await _repositorioDetVis.Obtener(u => u.IdReg == IdReg);
                if (det_encontrado == null)
                    throw new TaskCanceledException("El detalle no existe en la base de datos!");

                bool respuesta = await _repositorioDetVis.Eliminar(det_encontrado);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Visita> Detalle(int idVisita)
        {
            IQueryable<Visita> query = await _repositorioVisita.Consultar(p => p.IdVisita == idVisita);
            return query.Include(f => f.IdFincaNavigation)
                        .Include(p => p.IdPlanNavigation)
                        .Include(d => d.DetalleVisita)
                            .ThenInclude(a => a.IdActividadNavigation) // Incluir IdActividadNavigation en DetalleVisita
                        .First();
        }

    }
}





//public async Task<List<Visita>> Lista()  // Este proceso era para extraer manualmente la data del modelo, se cambio a VMde
//{
//    // Esperamos el resultado de Consultar para obtener el IQueryable<Visita>
//    IQueryable<Visita> query = await _repositorioVisita.Consultar();

//    // Luego aplicamos el Select sobre el IQueryable
//    var visitasConDatosSeleccionados = await query
//        .Select(visita => new Visita
//        {
//            IdVisita = visita.IdVisita,
//            IdFinca = visita.IdFinca,
//            IdPlan = visita.IdPlan,
//            Observaciones = visita.Observaciones,
//            Zafra = visita.Zafra,
//            Fecha = visita.Fecha,
//            SentTo = visita.SentTo,
//            Responsable = visita.Responsable,
//            Mandador = visita.Mandador,
//            Latitud = visita.Latitud,
//            Longitud = visita.Longitud,
//            Urlfoto1 = visita.Urlfoto1,
//            Urlfoto2 = visita.Urlfoto2,
//            Nombrefoto1 = visita.Nombrefoto1,
//            Nombrefoto2 = visita.Nombrefoto2,

//            // Seleccionar solo las propiedades de IdPlanNavigation necesarias
//            IdPlanNavigation = new PlanesTrabajo
//            {
//                IdPlan = visita.IdPlanNavigation.IdPlan,
//                Descripcion = visita.IdPlanNavigation.Descripcion,
//                FechaIni = visita.IdPlanNavigation.FechaIni,
//                FechaFin = visita.IdPlanNavigation.FechaFin,
//            },

//            // Seleccionar solo las propiedades de IdFincaNavigation necesarias
//            IdFincaNavigation = new MaestroFinca
//            {
//                IdFinca = visita.IdFincaNavigation.IdFinca,
//                CodFinca = visita.IdFincaNavigation.CodFinca,
//                Descripcion = visita.IdFincaNavigation.Descripcion,
//                Proveedor = visita.IdFincaNavigation.Proveedor,
//                Email = visita.IdFincaNavigation.Email,
//            },

//            // Seleccionar solo las propiedades de DetalleVisita necesarias
//            DetalleVisita = visita.DetalleVisita.Select(det => new DetalleVisita
//            {
//                IdReg = det.IdReg,
//                IdVisita = det.IdVisita,
//                IdFinca = det.IdFinca,
//                IdActividad = det.IdActividad,
//                Fecha = det.Fecha,
//                Avances = det.Avances,
//                Avanceanterior = det.Avanceanterior,
//                Estado = det.Estado,
//                Estadoanterior = det.Estadoanterior,
//                Comentarios = det.Comentarios,
//                Observaciones = det.Observaciones,
//                Urlfoto1 = det.Urlfoto1,
//                Nombrefoto1 = det.Nombrefoto1,

//                // Proyectar solo los campos necesarios de IdActividadNavigation
//                IdActividadNavigation = new Actividad
//                {
//                    IdActividad = det.IdActividadNavigation.IdActividad,
//                    Descripcion = det.IdActividadNavigation.Descripcion,
//                }
//            }).ToList()

//        }).ToListAsync();

//    return visitasConDatosSeleccionados;
//}