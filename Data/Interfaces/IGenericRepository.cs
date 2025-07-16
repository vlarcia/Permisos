using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IDbContextTransaction> IniciarTransaccionAsync();
        /// <returns>Entidad encontrada o null si no existe.</returns>
        Task<TEntity> Obtener(
            Expression<Func<TEntity, bool>> filtro,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);        
        Task<TEntity> Crear(TEntity entidad);
        Task<bool> Editar(TEntity entidad);
        Task<bool> Eliminar(TEntity entidad);
        IQueryable<TEntity> Consultar(Expression<Func<TEntity, bool>> filtro = null);
    }
}
