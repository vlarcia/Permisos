using Business.Interfaces;
using Data.Interfaces;
using Entity;
using Data.DBContext;
using Microsoft.CodeAnalysis.CSharp;
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
    public class DestinatarioService : IDestinatarioService
    {
        private readonly IGenericRepository<TbDestinatario> _repositorio;
        private readonly IGenericRepository<TbPermisoDestinatario> _reposPermisoDestino;
        private readonly PermisosContext _context;

        public DestinatarioService(IGenericRepository<TbDestinatario> repositorio, IGenericRepository<TbPermisoDestinatario> reposPermisoDestino, PermisosContext context)

        {
            _repositorio = repositorio;
            _reposPermisoDestino = reposPermisoDestino;
            _context = context;
        }
        public async Task<List<TbDestinatario>> Lista()
        {
            IQueryable<TbDestinatario> query = _repositorio.Consultar();            
            return await query.Include(a=> a.IdAreaNavigation).Include(l=> l.TbPermisoDestinatarios).ToListAsync();
        }
        public async Task<List<TbDestinatario>> ListaPorArea(int idArea)
        {
            IQueryable<TbDestinatario> query = _repositorio.Consultar();           
            if (idArea > 1)
            {
                // Filtra solo destinatarios de esa área
                query = query.Where(d => d.IdArea == idArea);
            }

            return await query.Include(a => a.IdAreaNavigation).Include(l => l.TbPermisoDestinatarios).ToListAsync();
        }
        
        public async Task<TbDestinatario> Crear(TbDestinatario entidad)
        {
            TbDestinatario destinatario_existe = await _repositorio.Obtener(u => (u.Correo == entidad.Correo ||
                                u.TelefonoWhatsapp==entidad.TelefonoWhatsapp)&& u.IdArea==entidad.IdArea);
            if (destinatario_existe != null)
                throw new TaskCanceledException("Ya existe un destinatario con ese correo o teléfono ");
            try
            {
                entidad.IdDestinatario = 0; // Asegurarse de que el ID sea cero para crear un nuevo registro    
                entidad.FechaRegistro = DateTime.Now;
                TbDestinatario destinatario_creado = await _repositorio.Crear(entidad);
                
                if (destinatario_creado.IdDestinatario == 0)
                    throw new TaskCanceledException("No se pudo crear el destinatario!");


                IQueryable<TbDestinatario> query = _repositorio.Consultar(u => u.IdDestinatario == destinatario_creado.IdDestinatario);
                
                destinatario_creado = await query.FirstAsync();

                return destinatario_creado;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TbDestinatario> Editar(TbDestinatario entidad)
        {
            TbDestinatario destinatario_existe = await _repositorio.Obtener(u => u.IdDestinatario == entidad.IdDestinatario);
            if (destinatario_existe == null)
                throw new TaskCanceledException("El Destinatario no existe");
            try
            {
                using (var transaction = await _repositorio.IniciarTransaccionAsync())
                {
                    try
                    {

                        destinatario_existe.Activo = entidad.Activo;
                        destinatario_existe.Correo = entidad.Correo;
                        destinatario_existe.TelefonoWhatsapp = entidad.TelefonoWhatsapp;
                        destinatario_existe.Nombre = entidad.Nombre;
                        destinatario_existe.RecibeAlta = entidad.RecibeAlta;
                        destinatario_existe.RecibeMedia = entidad.RecibeMedia;
                        destinatario_existe.RecibeBaja = entidad.RecibeBaja;
                        destinatario_existe.FechaRegistro = DateTime.Now;
                        destinatario_existe.IdArea = entidad.IdArea;

                        bool respuesta = await _repositorio.Editar(destinatario_existe);
                        if (!respuesta)
                            throw new TaskCanceledException("No se pudo actualizar el destinatario.  Revise !");
                        var permisosExistentes = await _reposPermisoDestino
                                .Consultar(p => p.IdDestinatario == entidad.IdDestinatario)
                                .AsNoTracking()
                                .ToListAsync();
                        // Recorrer los detalles de permisos editados del destinatario
                        foreach (var permisoEditado in entidad.TbPermisoDestinatarios)
                        {
                            if (permisoEditado.IdPermisoDestinatario == 0)  // Si este Id es 0, es un nuevo permiso que no existía
                            {
                                permisoEditado.Estado = true; // Asignar estado por defecto
                                permisoEditado.FechaAsignacion = DateTime.Now; // Asignar fecha de asignación
                                var respuestaPermiso = await _reposPermisoDestino.Crear(permisoEditado);
                                if (respuestaPermiso.IdPermisoDestinatario == 0)
                                    throw new TaskCanceledException("No se pudo crear el permiso-destinatario. Revise !");
                            }
                            else // Si el Id es diferente de 0, es un permiso que ya existía y se está conservando solo cambia la fecha de asignación y el estado
                            {
                                var permisoExistente = permisosExistentes.FirstOrDefault(p => p.IdPermisoDestinatario == permisoEditado.IdPermisoDestinatario);
                                if (permisoExistente != null)
                                {
                                    // Actualizar el permiso existente
                                    permisoExistente.Estado = true;
                                    permisoExistente.FechaAsignacion = DateTime.Now;
                                    bool respuestaPermiso = await _reposPermisoDestino.Editar(permisoExistente);
                                    if (!respuestaPermiso)
                                        throw new TaskCanceledException("No se pudo actualizar el permiso-destinatario. Revise !");
                                }
                            }
                        }
                        // Eliminar detalles que ya no están en la lista editada
                        foreach (var permisoExistente in permisosExistentes)
                        {
                            if (!entidad.TbPermisoDestinatarios.Any(p => p.IdPermisoDestinatario == permisoExistente.IdPermisoDestinatario))
                            {
                                // Si el permiso existente no está en la lista editada, eliminarlo
                                bool respuestaEliminar = await _reposPermisoDestino.Eliminar(permisoExistente);
                                if (!respuestaEliminar)
                                    throw new TaskCanceledException("No se pudo eliminar el permiso-destinatario. Revise !");
                            }
                        }
                        // Confirmar la transacción si todo ha ido bien
                        await transaction.CommitAsync();

                        // Retornar el destinatario editado con todos sus detalles
                        TbDestinatario destinatario_editado = await _repositorio.Obtener(u => u.IdDestinatario == entidad.IdDestinatario,
                            include: q => q.Include(d => d.TbPermisoDestinatarios)
                                            .Include(a => a.IdAreaNavigation));
                        return destinatario_editado;
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre un error, revertir la transacción
                        await transaction.RollbackAsync();
                        throw new TaskCanceledException("Error al editar el destinatario: " + ex.Message);
                    }
                }                    
            }
            catch (Exception ex)
            {
                throw new TaskCanceledException("Error al editar la compra: " + ex.Message);
            }
        }

        //public async Task<bool> Eliminar(int IdDestinatario)
        //{
        //    try
        //    {
        //        TbPermisoDestinatario permisoDestinatario_encontrado = await _reposPermisoDestino.Obtener(u => u.IdDestinatario == IdDestinatario);
        //        TbDestinatario destinatario_encontrado = await _repositorio.Obtener(u => u.IdDestinatario == IdDestinatario);
        //        if (destinatario_encontrado == null)
        //            throw new TaskCanceledException("El destinatario no existe!");

        //        bool respuesta = await _repositorio.Eliminar(destinatario_encontrado);
        //        return true;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public async Task<bool> Eliminar(int idDestinatario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Obtener el destinatario
                var destinatario = await _context.TbDestinatarios
                    .Include(d => d.TbPermisoDestinatarios)
                    .Include(d => d.TbAlerta)
                    .FirstOrDefaultAsync(d => d.IdDestinatario == idDestinatario);

                if (destinatario == null)
                    throw new Exception("Destinatario no encontrado.");

                // Eliminar los permisos asociados
                if (destinatario.TbPermisoDestinatarios.Any())
                    _context.TbPermisoDestinatarios.RemoveRange(destinatario.TbPermisoDestinatarios);

                // Eliminar las alertas asociadas
                if (destinatario.TbAlerta.Any())
                    _context.TbAlertas.RemoveRange(destinatario.TbAlerta);

                // Finalmente, eliminar el destinatario
                _context.TbDestinatarios.Remove(destinatario);

                // Guardar cambios y confirmar la transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al eliminar el destinatario: " + ex.Message);
            }
        }


        public async Task<TbDestinatario> ObtenerPorId(int iddestinatario)
        {
            try
            {
                TbDestinatario resultado = null;
                if (iddestinatario != 0)
                {
                    IQueryable<TbDestinatario> queryDestinatario = _repositorio.Consultar(u => u.IdDestinatario == iddestinatario);
                    resultado = await queryDestinatario.FirstAsync();
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw ex; // Capturar el error, pero sería bueno manejarlo más adecuadamente (ej. log)
            }

        }
        public async Task<List<TbPermisoDestinatario>> ListaPermisoDestinatario()
        {
            IQueryable<TbPermisoDestinatario> query = _reposPermisoDestino.Consultar();
            return await query.ToListAsync();
        }

    }
}
