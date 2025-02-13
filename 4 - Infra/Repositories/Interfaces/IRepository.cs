using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Infra.Repositories.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<TEntity> InsertAsync(TEntity entity);
        TEntity Insert(TEntity entity);
        TEntity Update(TEntity entity);
        void Remove(TEntity entity);
        Task DeleteAsync(int id);
        Task DeleteGuidAsync(Guid id);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
