using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data.DBContext;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Implementacion
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly CumplimientoContext _cumplimientoContext;
        public GenericRepository(CumplimientoContext cumplimiento)
        {
                _cumplimientoContext=cumplimiento;
        }
        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
        {
            try
            {
                TEntity entidad = await _cumplimientoContext.Set<TEntity>().FirstOrDefaultAsync(filtro);
                return entidad;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<TEntity> Crear(TEntity entidad)
        {
            try
            {
                _cumplimientoContext.Set<TEntity>().Add(entidad);
                await _cumplimientoContext.SaveChangesAsync();  
                return entidad;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _cumplimientoContext.Update(entidad);
                await _cumplimientoContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
  
        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _cumplimientoContext.Remove(entidad);
                await _cumplimientoContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            IQueryable<TEntity> queryEntidad = filtro == null? _cumplimientoContext.Set<TEntity>(): _cumplimientoContext.Set<TEntity>().Where(filtro);
            return queryEntidad;
        }

    }
}
