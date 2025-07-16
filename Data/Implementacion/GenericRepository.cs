using Data.DBContext;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementacion
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly PermisosContext _permisosContext;
        public GenericRepository(PermisosContext cumplimiento)
        {
                _permisosContext=cumplimiento;
        }
        // ✅ Método para iniciar una transacción correctamente
        public async Task<IDbContextTransaction> IniciarTransaccionAsync()
        {
            return await _permisosContext.Database.BeginTransactionAsync();
        }
      
        public async Task<TEntity> Obtener(
           Expression<Func<TEntity, bool>> filtro,
           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            try
            {
                IQueryable<TEntity> query = _permisosContext.Set<TEntity>();

                if (include != null)
                {
                    query = include(query);
                }

                return await query.AsNoTracking().FirstOrDefaultAsync(filtro);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Obtener: {ex.Message}");
                throw;
            }
        }
        // ✅ Método para crear una nueva entidad
        public async Task<TEntity> Crear(TEntity entidad)
        {
            try
            {
                _permisosContext.Set<TEntity>().Add(entidad);
                await _permisosContext.SaveChangesAsync();
                return entidad;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Crear: {ex.Message}");
                throw;
            }
        }

        // ✅ Método para editar una entidad existente
        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _permisosContext.Entry(entidad).State = EntityState.Modified;
                await _permisosContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Editar: {ex.Message}");
                throw;
            }
        }

        // ✅ Método para eliminar una entidad
        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _permisosContext.Set<TEntity>().Remove(entidad);
                await _permisosContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Eliminar: {ex.Message}");
                throw;
            }
        }

        // ✅ Método para consultar múltiples registros con opción de filtrado
        public IQueryable<TEntity> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            try
            {
                IQueryable<TEntity> query = _permisosContext.Set<TEntity>();

                if (filtro != null)
                {
                    query = query.Where(filtro);
                }

                return query.AsNoTracking(); // Optimización para consultas de solo lectura
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Consultar: {ex.Message}");
                throw;
            }
        }
    }
}
