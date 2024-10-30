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
    public class VisitaService : IVisitaService
    {
        private readonly IVisitaRepository _repositorioVisita;
        private readonly IGenericRepository<DetalleVisita> _repositorioDetVis;
        private readonly CumplimientoContext _dbContext;
        
        public VisitaService(IVisitaRepository repositorioVisita, IGenericRepository<DetalleVisita> repositorioDetVis,
                            CumplimientoContext dbContext)
            
        {
            _repositorioVisita = repositorioVisita;
            _repositorioDetVis = repositorioDetVis;               
            _dbContext = dbContext;
        }

        public async Task<List<Visita>> Lista()
        {         
            IQueryable<Visita> query = await _repositorioVisita.Consultar();
            IQueryable<DetalleVisita> query2 = await _repositorioDetVis.Consultar();

            List<Visita> lalista = query.Include(p => p.IdFincaNavigation)
                        .Include(d => d.DetalleVisita)
                        .ToList();
            return lalista;         
        }
            public async Task<Visita> Crear(Visita entidad)
        {
            try
            {
                Visita visita_creada = await _repositorioVisita.Registrar(entidad);  //Registrar esta en su repsitorio propio
                if (visita_creada.IdVisita == 0)
                    throw new TaskCanceledException("No se puedo crear la Visita!");

                IQueryable<Visita> query = await _repositorioVisita.Consultar(u => u.IdVisita == visita_creada.IdVisita);
                visita_creada = query.First();

                return visita_creada;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Visita> Editar(Visita entidad)
        {
            Visita visita_existe = await _repositorioVisita.Obtener(p => p.IdVisita == entidad.IdVisita);
            if (visita_existe == null)
                throw new TaskCanceledException("La Visita no existe en la base de datos, no puede ser editado!");
            try
            {
                IQueryable<Visita> queryPlan = await _repositorioVisita.Consultar(p => p.IdVisita == entidad.IdVisita);
                Visita visita_editada = queryPlan.First();
                visita_editada.Observaciones = entidad.Observaciones;
                visita_editada.IdFinca = entidad.IdFinca;
                visita_editada.IdPlan = entidad.IdPlan;
                visita_editada.Zafra = entidad.Zafra;
                visita_editada.AndroidId = entidad.AndroidId;
                visita_editada.Responsable = entidad.Responsable;
                visita_editada.Mandador = entidad.Mandador;
                visita_editada.Longitud = entidad.Longitud;
                visita_editada.Latitud = entidad.Latitud;
                visita_editada.Fecha = entidad.Fecha;
                

                foreach (var detalle in entidad.DetalleVisita)
                {
                    IQueryable<DetalleVisita> queryActividad = await _repositorioDetVis.Consultar(p => p.IdReg == detalle.IdReg);
                    DetalleVisita detalleeditado = queryActividad.First();
                    detalleeditado.Fecha = detalle.Fecha;
                    detalleeditado.Avances = detalle.Avances;
                    detalleeditado.Avanceanterior = detalle.Avanceanterior;
                    detalleeditado.Estado = detalle.Estado;
                    detalleeditado.Estadoanterior = detalle.Estadoanterior;
                    detalleeditado.Comentarios = detalle.Comentarios;
                    detalleeditado.Observaciones = detalle.Observaciones;

                    bool respuesta2 = await _repositorioDetVis.Editar(detalleeditado);
                    if (!respuesta2)
                        throw new TaskCanceledException("Hubo problemas en el detalle de la Visita y no se pudo editar!");
                }


                bool respuesta = await _repositorioVisita.Editar(visita_editada);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar la Visita.   Revise!");
                Visita visita_fueeditada = queryPlan.First();
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

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                                        
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
    }
}
