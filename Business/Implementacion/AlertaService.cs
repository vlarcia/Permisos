using Business.Interfaces;
using Data.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Business.Implementacion
{
    public class AlertaService : IAlertaService
    {
        private readonly IGenericRepository<TbAlerta> _repositorio;        
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;
        public AlertaService(IGenericRepository<TbAlerta> repositorio)
        {
            _repositorio = repositorio;                                    

        }
        public async Task<List<TbAlerta>> Lista()
        {
            IQueryable<TbAlerta> query = _repositorio.Consultar();
            return await query.Include(a => a.IdDestinatarioNavigation).Include(a => a.IdPermisoNavigation)
                    .OrderByDescending(f => f.FechaEnvio).ToListAsync();
        }
        public async Task<TbAlerta> Crear(TbAlerta entidad, bool permitirDuplicado = false)
        {
            DateTime fechaLimite = DateTime.Now.AddDays(-12);

            // Validar si ya existe alerta reciente
            TbAlerta alertaExistente = await _repositorio.Obtener(u =>
                u.IdPermiso == entidad.IdPermiso &&
                u.IdDestinatario == entidad.IdDestinatario &&
                u.FechaEnvio >= fechaLimite);

            if (alertaExistente != null)
            {
                if (!permitirDuplicado)
                    throw new TaskCanceledException("Ya se generó una alerta para este permiso y destinatario en los últimos 12 días.");

                // ✅ Retornar alerta existente si es reenviada manualmente
                return alertaExistente;
            }

            try
            {
                TbAlerta alertaCreada = await _repositorio.Crear(entidad);
                if (alertaCreada.IdAlerta == 0)
                    throw new TaskCanceledException("No se pudo crear la alerta!");

                IQueryable<TbAlerta> query = _repositorio.Consultar(u => u.IdAlerta == alertaCreada.IdAlerta);
                alertaCreada = await query.FirstAsync();

                return alertaCreada;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<TbAlerta> Editar(TbAlerta entidad)
        {
            TbAlerta alerta_existe = await _repositorio.Obtener(u => u.IdAlerta != entidad.IdAlerta);
            if (alerta_existe != null)
                throw new TaskCanceledException("El correo ya existe");
            try
            {
                IQueryable<TbAlerta> queryAlerta = _repositorio.Consultar(u => u.IdAlerta == entidad.IdAlerta);
                TbAlerta alerta_editar = await queryAlerta.FirstAsync();
                alerta_editar.Mensaje = entidad.Mensaje;
                alerta_editar.Resultado = entidad.Resultado;
                alerta_editar.FechaEnvio = entidad.FechaEnvio;
                alerta_editar.IdDestinatario = entidad.IdDestinatario;
                alerta_editar.IdPermiso = entidad.IdPermiso;
                alerta_editar.MedioEnvio = entidad.MedioEnvio;
                alerta_editar.Resultado = entidad.Resultado;

                bool respuesta = await _repositorio.Editar(alerta_editar);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo actualizar la alerta.  Revise !");
                TbAlerta alerta_editada = queryAlerta.Include(r => r.IdPermisoNavigation).First();
                return alerta_editada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int IdAlerta)
        {
            try
            {
                TbAlerta alerta_encontrada = await _repositorio.Obtener(u => u.IdAlerta == IdAlerta);
                if (alerta_encontrada == null)
                    throw new TaskCanceledException("La alerta no existe!");

                bool respuesta = await _repositorio.Eliminar(alerta_encontrada);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public async Task<TbAlerta> ObtenerPorId(int idalerta)
        {
            try
            {
                TbAlerta resultado = null;
                if (idalerta != 0)
                {
                    IQueryable<TbAlerta> queryAlerta = _repositorio.Consultar(u => u.IdAlerta == idalerta);
                    resultado = await queryAlerta.Include(a => a.IdDestinatarioNavigation).Include(p => p.IdPermisoNavigation).FirstAsync();

                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw ex; // Capturar el error, pero sería bueno manejarlo más adecuadamente (ej. log)
            }
        }
        public async Task<bool> ExisteAlertaRecienteAsync(int idPermiso, int idDestinatario)
        {
            try
            {
                var query = _repositorio.Consultar(a =>
                    a.IdPermiso == idPermiso &&
                    a.IdDestinatario == idDestinatario);

                var ultimaAlerta = await query
                    .OrderByDescending(a => a.FechaEnvio)
                    .FirstOrDefaultAsync();

                if (ultimaAlerta == null)
                    return false; // Nunca se ha enviado, entonces se puede enviar

                // Si han pasado más de 10 días desde el último envío, se puede enviar de nuevo
                return (DateTime.Now - ultimaAlerta.FechaEnvio).TotalDays < 12;
            }
            catch
            {
                return true; // Si algo falla, asumir que se puede enviar
            }
        }



    }
}
