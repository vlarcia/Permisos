using System;
using System.Collections.Generic;
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
    public class SincronizaService : ISincronizaService
    {
        private readonly IGenericRepository<AndroidId> _repositorioAndroid;
        private readonly IVisitaRepository _repositorioVisita;
        private readonly IGenericRepository<Actividad> _repositorioActividad;
        private readonly CumplimientoContext _dbContext;
        public SincronizaService(IGenericRepository<AndroidId> repositorioAndroid, IVisitaRepository repositorioVisita,
                                IGenericRepository<Actividad> repositorioActividad, CumplimientoContext dbContext)
        {
            _repositorioAndroid = repositorioAndroid;
            _dbContext = dbContext;
            _repositorioActividad = repositorioActividad;
            _repositorioVisita = repositorioVisita;
        }            

        public async Task<List<AndroidId>> ListaAndroid()
        {
           IQueryable<AndroidId> query = await _repositorioAndroid.Consultar();
            return query.ToList();  
        }
        public async Task<List<Visita>> ListaVisita()
        {
            IQueryable<Visita> query = await _repositorioVisita.Consultar();
            return query.Include(f => f.IdFincaNavigation)
                            .Include(p => p.IdPlanNavigation)
                            .Include(d => d.DetalleVisita)
                            .ThenInclude(a => a.IdActividadNavigation)
                            .Where(s=> s.Aplicado!=true)
                            .ToList();
        }

        public async Task<AndroidId> Crear(AndroidId entidad)
        {
            AndroidId android_existe = await _repositorioAndroid.Obtener(a => a.IdAndroid == entidad.IdAndroid);
            if (android_existe != null)
                throw new TaskCanceledException("El Dispositivo Android ya existe");
            try
            {
                AndroidId android_creado = await _repositorioAndroid.Crear(entidad);
                if (android_creado.IdAndroid == 0)
                    throw new TaskCanceledException("No se pudo crear el nuevo dispositivo Android!");

                IQueryable<AndroidId> query = await _repositorioAndroid.Consultar(a => a.IdAndroid == android_creado.IdAndroid);
                android_creado = query.First();

                return android_creado;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<AndroidId> Editar(AndroidId entidad)
        {
            AndroidId android_existe = await _repositorioAndroid.Obtener(a => a.IdAndroid == entidad.IdAndroid);
            if (android_existe == null)
                throw new TaskCanceledException("El codigo de dispositivo Android no existe!");
            try
            {
                IQueryable<AndroidId> queryAndroid = await _repositorioAndroid.Consultar(a => a.IdAndroid == entidad.IdAndroid);
                AndroidId android_editar = queryAndroid.First();
                android_editar.Androidid1 = entidad.Androidid1;
                android_editar.Dispositivo = entidad.Dispositivo;
                android_editar.Responsable = entidad.Responsable;
                android_editar.Email = entidad.Email;                

                bool respuesta = await _repositorioAndroid.Editar(android_editar);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo actualizar el Dispositivo Android.  Revise !");
                
                return android_editar;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int IdAndroid)
        {
            try
            {
                AndroidId android_encontrado = await _repositorioAndroid.Obtener(a => a.IdAndroid == IdAndroid);
                if (android_encontrado == null)
                    throw new TaskCanceledException("El dispositivo Android no existe!");

                bool respuesta = await _repositorioAndroid.Eliminar(android_encontrado);
                 return respuesta; 
                
            }
            catch
            {
                throw;
            }
        }
      
        public async Task<Visita> AplicaVisita(Visita entidad)
        {
            try
            {
                IQueryable<Visita> queryVisita = await _repositorioVisita.Consultar(v => v.IdVisita == entidad.IdVisita);
                Visita visita_aplicar = queryVisita
                                        .Include(p=> p.IdPlanNavigation)
                                        .Include(f=> f.IdFincaNavigation)
                                        .First();

                if (queryVisita == null)
                    throw new TaskCanceledException("La Visita no existe en la base de datos, no puede ser aplicada al plan!");
                
                visita_aplicar.Aplicado = true;
                
                foreach (var detalle in entidad.DetalleVisita)
                {
                    IQueryable<Actividad> queryActividad = await _repositorioActividad.Consultar(p => p.IdActividad == detalle.IdActividad);
                    Actividad actividadeditar = queryActividad.First();
                    if ( !actividadeditar.FechaUltimarevision.HasValue || actividadeditar.FechaUltimarevision < detalle.Fecha )
                    {                        
                        actividadeditar.FechaUltimarevision = detalle.Fecha;
                        actividadeditar.Avances = detalle.Avances;
                        actividadeditar.Avanceanterior = detalle.Avanceanterior;
                        actividadeditar.Estado = detalle.Estado;                        
                        actividadeditar.Comentarios = detalle.Comentarios;                        

                        bool respuesta2 = await _repositorioActividad.Editar(actividadeditar);
                        if (!respuesta2)
                            throw new TaskCanceledException("Hubo problemas al tratar de aplicar la actividad.  Revise Planes y Visitas asociadas!!");
                    }
                    else
                    {
                        throw new TaskCanceledException("La fecha de la Visita es menor a la fecha de la última revision de las Actividades o nula.  Revise!!");
                    }
                }

                bool respuesta = await _repositorioVisita.Editar(visita_aplicar);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo poner la visita en modo Aplicado.   Revise!");
                
                return visita_aplicar;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }           
}
