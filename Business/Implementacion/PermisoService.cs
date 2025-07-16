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
    public class PermisoService : IPermisoService
    {
        private readonly IGenericRepository<TbPermiso> _repositorio;
        private readonly IFireBaseService _firebaseService;

        public PermisoService(IGenericRepository<TbPermiso> repositorio, IFireBaseService firebaseService)

        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
        }
        public async Task<List<TbPermiso>> Lista()
        {
            IQueryable<TbPermiso> query = _repositorio.Consultar();
            return await query.ToListAsync();
        }

        public async Task<List<TbPermiso>> ListaPorArea(int idArea)
        {
            IQueryable<TbPermiso> query = _repositorio.Consultar();
            if (idArea > 1)
            {
                // 1 es administardor, si area es mayor de 1 Filtra solo destinatarios de esa área 
                query = query.Where(d => d.IdArea == idArea);
            }

            return await query.Include(a => a.IdAreaNavigation).ToListAsync();
        }

        public async Task<TbPermiso> Crear(TbPermiso entidad, Stream archivoEvidencia = null, string nombreEvidencia = "",
                                                              Stream archivoEvidencia2 = null, string nombreEvidencia2 = "",
                                                              Stream archivoEvidencia3 = null, string nombreEvidencia3 = "")
        {
            TbPermiso permiso_existe = await _repositorio.Obtener(u => u.Nombre == entidad.Nombre);
            if (permiso_existe != null)
                throw new TaskCanceledException("Ya existe un permiso con ese mismo Nombre!");
            try
            {
                entidad.IdPermiso = 0; // Asegurarse de que el ID sea cero para crear un nuevo registro              
                if (archivoEvidencia != null)
                {
                    string UrlEvidencia = await _firebaseService.SubirStorage(archivoEvidencia, "carpeta_evidencia", nombreEvidencia);
                    entidad.NombreEvidencia2 = nombreEvidencia;
                    entidad.UrlEvidencia2 = UrlEvidencia;
                }

                if (archivoEvidencia2 != null)
                {
                    string UrlEvidencia2 = await _firebaseService.SubirStorage(archivoEvidencia2, "carpeta_evidencia", nombreEvidencia2);
                    entidad.NombreEvidencia2 = nombreEvidencia2;
                    entidad.UrlEvidencia2 = UrlEvidencia2;
                }
                if (archivoEvidencia3 != null)
                {
                    string UrlEvidencia3 = await _firebaseService.SubirStorage(archivoEvidencia3, "carpeta_evidencia", nombreEvidencia3);
                    entidad.NombreEvidencia3 = nombreEvidencia3;
                    entidad.UrlEvidencia3 = UrlEvidencia3;
                }

                TbPermiso permiso_creado = await _repositorio.Crear(entidad);
                               
                if (permiso_creado.IdPermiso == 0)
                    throw new TaskCanceledException("No se pudo crear el permiso!");

               

                IQueryable<TbPermiso> query = _repositorio.Consultar(u => u.IdPermiso == permiso_creado.IdPermiso);
                permiso_creado = await query.FirstAsync();

                return permiso_creado;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TbPermiso> Editar(TbPermiso entidad, Stream archivoEvidencia = null, string nombreEvidencia = "",
                                                               Stream archivoEvidencia2 = null, string nombreEvidencia2 = "",
                                                               Stream archivoEvidencia3 = null, string nombreEvidencia3 = "",
                                                               bool eliminar1 = false,
                                                               bool eliminar2 = false,
                                                               bool eliminar3 = false)
        {
            TbPermiso permiso_existe = await _repositorio.Obtener(u => u.IdPermiso == entidad.IdPermiso);
            if (permiso_existe == null)
                throw new TaskCanceledException("El Permiso no existe");
            try
            {

                if (eliminar1 && !string.IsNullOrEmpty(permiso_existe.NombreEvidencia))
                {
                    await _firebaseService.EliminarStorage("carpeta_evidencia", permiso_existe.NombreEvidencia);
                    permiso_existe.NombreEvidencia = null;
                    permiso_existe.UrlEvidencia = null;
                }
                else if (archivoEvidencia != null)
                {
                    if (!string.IsNullOrEmpty(permiso_existe.NombreEvidencia) && permiso_existe.NombreEvidencia != entidad.NombreEvidencia)
                    {
                        await _firebaseService.EliminarStorage("carpeta_evidencia", permiso_existe.NombreEvidencia);
                    }
                    string laUrlEvidencia = await _firebaseService.SubirStorage(archivoEvidencia, "carpeta_evidencia", nombreEvidencia);
                    entidad.NombreEvidencia = nombreEvidencia;
                    entidad.UrlEvidencia = laUrlEvidencia;
                }
                else if (archivoEvidencia == null && !eliminar1)
                {
                    entidad.NombreEvidencia = permiso_existe.NombreEvidencia;
                    entidad.UrlEvidencia = permiso_existe.UrlEvidencia;
                }

                ///=======================   Archivo 2 ==========================
                if (eliminar2 && !string.IsNullOrEmpty(permiso_existe.NombreEvidencia2))
                {
                    await _firebaseService.EliminarStorage("carpeta_evidencia", permiso_existe.NombreEvidencia2);
                    permiso_existe.NombreEvidencia2 = null;
                    permiso_existe.UrlEvidencia2 = null;
                }
                else if (archivoEvidencia2 != null)
                {
                    if (!string.IsNullOrEmpty(permiso_existe.NombreEvidencia2) && permiso_existe.NombreEvidencia2 != entidad.NombreEvidencia2)
                    {
                        await _firebaseService.EliminarStorage("carpeta_evidencia", permiso_existe.NombreEvidencia2);
                    }
                    string laUrlEvidencia2 = await _firebaseService.SubirStorage(archivoEvidencia2, "carpeta_evidencia", nombreEvidencia2);
                    entidad.NombreEvidencia2 = nombreEvidencia2;
                    entidad.UrlEvidencia2 = laUrlEvidencia2;
                }
                else if (archivoEvidencia2 == null && !eliminar2)
                {
                    entidad.NombreEvidencia2 = permiso_existe.NombreEvidencia2;
                    entidad.UrlEvidencia2 = permiso_existe.UrlEvidencia2;
                }


                ///=======================   Archivo 3 ==========================
                if (eliminar3 && !string.IsNullOrEmpty(permiso_existe.NombreEvidencia3))
                {
                    await _firebaseService.EliminarStorage("carpeta_evidencia", permiso_existe.NombreEvidencia3);
                    permiso_existe.NombreEvidencia3 = null;
                    permiso_existe.UrlEvidencia3 = null;
                }
                else if (archivoEvidencia3 != null)
                {
                    if (!string.IsNullOrEmpty(permiso_existe.NombreEvidencia3) && permiso_existe.NombreEvidencia3 != entidad.NombreEvidencia3)
                    {
                        await _firebaseService.EliminarStorage("carpeta_evidencia", permiso_existe.NombreEvidencia3);
                    }
                    string laUrlEvidencia3 = await _firebaseService.SubirStorage(archivoEvidencia3, "carpeta_evidencia", nombreEvidencia3);
                    entidad.NombreEvidencia3 = nombreEvidencia3;
                    entidad.UrlEvidencia3 = laUrlEvidencia3;
                }
                else if (archivoEvidencia3 == null && !eliminar3)
                {
                    entidad.NombreEvidencia3 = permiso_existe.NombreEvidencia3;
                    entidad.UrlEvidencia3 = permiso_existe.UrlEvidencia3;
                }


                IQueryable<TbPermiso> queryAlerta = _repositorio.Consultar(u => u.IdPermiso == entidad.IdPermiso);
                TbPermiso permiso_editar = await queryAlerta.FirstAsync();
                permiso_editar.Criticidad = entidad.Criticidad;
                permiso_editar.Descripcion = entidad.Descripcion;
                permiso_editar.Nombre = entidad.Nombre;
                permiso_editar.Encargado = entidad.Encargado;
                permiso_editar.Institucion = entidad.Institucion;
                permiso_editar.FechaVencimiento = entidad.FechaVencimiento;
                permiso_editar.DiasGestion = entidad.DiasGestion;
                permiso_editar.Tipo = entidad.Tipo;
                permiso_editar.EstadoPermiso = entidad.EstadoPermiso;
                permiso_editar.FechaModificacion = entidad.FechaModificacion;
                permiso_editar.FechaCreacion = entidad.FechaCreacion;
                permiso_editar.FechaVencimiento = entidad.FechaVencimiento;
                permiso_editar.IdArea = entidad.IdArea;
                permiso_editar.NombreEvidencia = entidad.NombreEvidencia;
                permiso_editar.UrlEvidencia = entidad.UrlEvidencia;
                permiso_editar.NombreEvidencia2 = entidad.NombreEvidencia2;
                permiso_editar.UrlEvidencia2 = entidad.UrlEvidencia2;
                permiso_editar.NombreEvidencia3 = entidad.NombreEvidencia3;
                permiso_editar.UrlEvidencia3 = entidad.UrlEvidencia3;

                bool respuesta = await _repositorio.Editar(permiso_editar);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo actualizar el permiso.  Revise !");
                TbPermiso permiso_editado = queryAlerta.First();
                return permiso_editado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int IdPermiso)
        {
            try
            {
                TbPermiso permiso_encontrado = await _repositorio.Obtener(u => u.IdPermiso == IdPermiso);
                if (permiso_encontrado == null)
                    throw new TaskCanceledException("El permiso no existe!");

                string nombreEvidencia = permiso_encontrado.NombreEvidencia;
                string nombreEvidencia2 = permiso_encontrado.NombreEvidencia2;
                string nombreEvidencia3 = permiso_encontrado.NombreEvidencia3;
                bool respuesta = await _repositorio.Eliminar(permiso_encontrado);
                if (respuesta)
                    await _firebaseService.EliminarStorage("carpeta_evidencia", nombreEvidencia);
                await _firebaseService.EliminarStorage("carpeta_evidencia", nombreEvidencia2);
                await _firebaseService.EliminarStorage("carpeta_evidencia", nombreEvidencia3);
                return true;                
            }
            catch
            {
                throw;
            }
        }
        public async Task<TbPermiso> ObtenerPorId(int idpermiso)
        {
            try
            {
                TbPermiso resultado = null;
                if (idpermiso != 0)
                {
                    IQueryable<TbPermiso> queryPermiso = _repositorio.Consultar(u => u.IdPermiso == idpermiso);
                    resultado = await queryPermiso.FirstAsync();
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
