using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace Business.Implementacion
{
    public class PlanesTrabajoService : IPlanesTrabajoService
    {
        private readonly IGenericRepository<Actividad> _repositorioactividad;
        private readonly IPlanesRepository _repositorioplanes;
        public PlanesTrabajoService(IGenericRepository<Actividad> repositorioactividad, IPlanesRepository repositorioplanes)
        {
            _repositorioactividad = repositorioactividad;
            _repositorioplanes = repositorioplanes;
        }


        public async Task<List<PlanesTrabajo>> Lista()
        {
            IQueryable<PlanesTrabajo> query = await _repositorioplanes.Consultar();
            return query.Include(p => p.IdFincaNavigation)
                        .Include(a => a.Actividades)
                        .ToList();
        }
        public async Task<List<Actividad>> ListaActividad()
        {
            IQueryable<Actividad> query = await _repositorioactividad.Consultar();
            return query.Include(f => f.IdFincaNavigation)
                        .Include(p => p.IdPlanNavigation)
                        .ToList();
        }

        public async Task<PlanesTrabajo> Registrar(PlanesTrabajo entidad)
        {
            try
            {
                PlanesTrabajo plan_creado = await _repositorioplanes.Registrar(entidad);
                if (plan_creado.IdPlan == 0)
                    throw new TaskCanceledException("No se puedo crear el plan!");

                IQueryable<PlanesTrabajo> query = await _repositorioplanes.Consultar(u => u.IdPlan == plan_creado.IdPlan);
                plan_creado = query.First();

                return plan_creado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PlanesTrabajo> Editar(PlanesTrabajo entidad)
        {
            PlanesTrabajo plan_existe = await _repositorioplanes.Obtener(p => p.IdPlan == entidad.IdPlan);
            if (plan_existe == null)
                throw new TaskCanceledException("El Plan no existe en la base de datos, no puede ser editado!");
            try
            {
                IQueryable<PlanesTrabajo> queryPlan = await _repositorioplanes.Consultar(p => p.IdPlan == entidad.IdPlan);
                PlanesTrabajo plan_editado = queryPlan.First();
                plan_editado.Descripcion = entidad.Descripcion;
                plan_editado.IdFinca = entidad.IdFinca;
                plan_editado.Estado = entidad.Estado;
                plan_editado.Avance = entidad.Avance;
                plan_editado.Observaciones = entidad.Observaciones;
                plan_editado.FechaIni = entidad.FechaIni;
                plan_editado.FechaFin = entidad.FechaFin;

                foreach (var actividad in entidad.Actividades)
                {
                    IQueryable<Actividad> queryActividad = await _repositorioactividad.Consultar(p => p.IdActividad == actividad.IdActividad);
                    Actividad actividadeditada = queryActividad.First();
                    actividadeditada.Descripcion = actividad.Descripcion;
                    actividadeditada.Tipo = actividad.Tipo;
                    actividadeditada.Recursos = actividad.Recursos;
                    actividadeditada.Responsable = actividad.Responsable;
                    actividadeditada.Avances = actividad.Avances;
                    actividadeditada.IdFinca = actividad.IdFinca;
                    actividadeditada.FechaIni = actividad.FechaIni;
                    actividadeditada.FechaFin = actividad.FechaFin;
                    actividadeditada.Comentarios = actividad.Comentarios;
                    actividadeditada.IdRequisito = actividad.IdRequisito;

                    bool respuesta2 = await _repositorioactividad.Editar(actividadeditada);
                    if (!respuesta2)
                        throw new TaskCanceledException("Hubo problemas en Actividades y no se pudo editar el plan!");
                }


                bool respuesta = await _repositorioplanes.Editar(plan_editado);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el plan.   Revise!");
                PlanesTrabajo plan_fueeditado = queryPlan
                                                   .Include(f=> f.IdFincaNavigation)
                                                   .Include(a => a.Actividades)
                                                    .First();
                return plan_fueeditado;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PlanesTrabajo>> Historial(int idPlan, string fechainicio, string fechafin)
        {
            IQueryable<PlanesTrabajo> query = await _repositorioplanes.Consultar();
            fechainicio = fechainicio is null ? "" : fechainicio;
            fechafin = fechafin is null ? "" : fechafin;
            if (fechainicio != "" && fechafin != "")
            {
                DateTime fec_inicio = DateTime.ParseExact(fechainicio, "dd/MM/yyyy", new CultureInfo("es-NI"));
                DateTime fec_fin = DateTime.ParseExact(fechafin, "dd/MM/yyyy", new CultureInfo("es-NI"));
                return query.Where(p =>
                p.FechaIni.Value.Date >= fec_inicio.Date &&
                p.FechaFin.Value.Date <= fec_fin.Date)

                .Include(act => act.Actividades)
                .ToList();
            }
            else
            {
                return query.Where(p =>
                p.IdPlan == idPlan)
                .Include(act => act.Actividades)
                .ToList();
            }

        }

        public async Task<PlanesTrabajo> Detalle(int idPlan)
        {
            IQueryable<PlanesTrabajo> query = await _repositorioplanes.Consultar(p => p.IdPlan == idPlan);
            return query.Include(p => p.IdFincaNavigation)
                        .Include(a => a.Actividades)
                        .First();
        }

        public async Task<List<Actividad>> Reporte(string fechainicio, string fechafin)
        {
            DateTime fec_inicio = DateTime.ParseExact(fechainicio, "dd/MM/yyyy", new CultureInfo("es-NI"));
            DateTime fec_fin = DateTime.ParseExact(fechafin, "dd/MM/yyyy", new CultureInfo("es-NI"));

            List<Actividad> lista = await _repositorioplanes.Reporte(fec_inicio, fec_fin);

            return lista;
        }

        public async Task<Actividad> EditarActividad(Actividad entidad)
        {
            Actividad act_existe = await _repositorioactividad.Obtener(p => p.IdActividad == entidad.IdActividad);
            if (act_existe == null)
                throw new TaskCanceledException("La Actividad no existe en la base de datos, no puede ser editada!");
            try
            {
                IQueryable<Actividad> queryActividad = await _repositorioactividad.Consultar(p => p.IdActividad == entidad.IdActividad);
                Actividad actividadeditada = queryActividad.First();
                actividadeditada.IdPlan = entidad.IdPlan;
                actividadeditada.IdRequisito = entidad.IdRequisito;
                actividadeditada.Descripcion = entidad.Descripcion;
                actividadeditada.Tipo = entidad.Tipo;
                actividadeditada.FechaIni = entidad.FechaIni;
                actividadeditada.FechaFin = entidad.FechaFin;
                actividadeditada.Responsable = entidad.Responsable;
                actividadeditada.Recursos = entidad.Recursos;
                actividadeditada.Avances = entidad.Avances;
                actividadeditada.Avanceanterior = entidad.Avanceanterior;
                actividadeditada.Estado = entidad.Estado;
                actividadeditada.Comentarios = entidad.Comentarios;
                actividadeditada.FechaUltimarevision = entidad.FechaUltimarevision;
                actividadeditada.IdFinca = entidad.IdFinca;

                bool respuesta = await _repositorioactividad.Editar(actividadeditada);
                if (!respuesta)
                    throw new TaskCanceledException("Hubo problemas en Actividades y no se pudo editar el plan!");
                Actividad act_fueeditada = queryActividad.First();
                return act_fueeditada;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Actividad> RegistrarActividad(Actividad entidad)
        {
            try
            {
                Actividad act_creada = await _repositorioactividad.Crear(entidad);
                if (act_creada.IdActividad == 0)
                    throw new TaskCanceledException("No se puede crear la actividad del plan!");

                IQueryable<Actividad> query = await _repositorioactividad.Consultar(u => u.IdActividad == act_creada.IdActividad);
                Actividad act_insertada = query.First();

                return act_insertada;

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<bool> Eliminar(int IdPlan)
        {
            try
            {
                Actividad hay_actividad = await _repositorioactividad.Obtener(u => u.IdPlan == IdPlan);
                if (hay_actividad != null)
                    throw new TaskCanceledException("Hay actividades asociadas a este plan!   No se puede eliminar hasta eliminar las actividades!");

                PlanesTrabajo plan_encontrado = await _repositorioplanes.Obtener(u => u.IdPlan == IdPlan);

                if (plan_encontrado == null)
                    throw new TaskCanceledException("El Plan no existe en la base de datos!");

                bool respuesta = await _repositorioplanes.Eliminar(plan_encontrado);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> EliminarActividad(int IdActividad)
        {
            try
            {
                Actividad act_encontrada = await _repositorioactividad.Obtener(u => u.IdActividad == IdActividad);
                if (act_encontrada == null)
                    throw new TaskCanceledException("La Actividad no existe en la base de datos!");

                bool respuesta = await _repositorioactividad.Eliminar(act_encontrada);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}


//async Task<List<CheckList>> IPlanesTrabajoService.ObtenerRequisito(string busqueda)
//{
//    IQueryable<CheckList> query = await _repositoriorequisito.Consultar(c=>
//    c.Descripcion.Contains(busqueda));
//    return query.ToList();
//}