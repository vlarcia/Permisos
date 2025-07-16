using Business.Implementacion;
using Business.Interfaces;
using Data.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementacion
{

    public class AreaService : IAreaService
    {
        private readonly IGenericRepository<TbArea> _repositorio;
     

        public AreaService(IGenericRepository<TbArea> repositorio)
        {
            _repositorio = repositorio;
            
        }
        public async Task<List<TbArea>> Lista()
        {
            IQueryable<TbArea> query = _repositorio.Consultar();
            return await query.ToListAsync();
        }
        
        public async Task<TbArea> Crear(TbArea entidad)
        {
            TbArea area_existe = await _repositorio.Obtener(u => u.Nombre == entidad.Nombre);
            if (area_existe != null)
                throw new TaskCanceledException("Ya existe un area con ese mismo Nombre!");
            try
            {
                entidad.IdArea = 0; // Asegurarse de que el ID sea cero para crear un nuevo registro
                entidad.FechaRegistro = DateTime.Now;
                TbArea area_creada = await _repositorio.Crear(entidad);
                if (area_creada.IdArea == 0)
                    throw new TaskCanceledException("No se pudo crear el area!");
                

                IQueryable<TbArea> query = _repositorio.Consultar(u => u.IdArea == area_creada.IdArea);
                return await query.FirstAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TbArea> Editar(TbArea entidad)
        {
            TbArea area_existe = await _repositorio.Obtener(u => u.IdArea == entidad.IdArea);
            if (area_existe == null)
                throw new TaskCanceledException("El Area no existe");
            try
            {
                IQueryable<TbArea> queryAlerta = _repositorio.Consultar(u => u.IdArea == entidad.IdArea);
                TbArea area_editar = await queryAlerta.FirstAsync();
                
                area_editar.Descripcion = entidad.Descripcion;
                area_editar.Nombre = entidad.Nombre;
                area_editar.Activo = entidad.Activo;

                bool respuesta = await _repositorio.Editar(area_editar);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo actualizar el area.  Revise !");
                TbArea area_editada = queryAlerta.First();
                return area_editada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int IdArea)
        {
            try
            {
                TbArea area_encontrada = await _repositorio.Obtener(u => u.IdArea == IdArea);
                if (area_encontrada == null)
                    throw new TaskCanceledException("El área no existe!");

                bool respuesta = await _repositorio.Eliminar(area_encontrada);
                return true;
            }
            catch
            {
                throw;
            }
        }

       
    }
}