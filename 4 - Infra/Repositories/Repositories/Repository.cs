using API.Infra.Repositories.Interfaces;
using API.Infra.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Infra.Repositories.Repositories
{
    internal class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly IDbContext _context;
        private DbSet<T> _entities;
        private bool _disposed = false;

        public Repository(IDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _entities;
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _entities;
            var first = await query.FirstOrDefaultAsync(predicate);
            return first;
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _entities.Where(predicate);
        }

        public async Task<T> InsertAsync(T entity)
        {
            var result = await _entities.AddAsync(entity);
            return result.Entity;
        }

        public T Insert(T entity)
        {
            var result = _entities.Add(entity);
            return result.Entity;
        }

        public T Update(T entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;

            return entity;
        }

        public void Remove(T entity)
        {
            if (entity != null)
                _entities.Remove(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _entities.FindAsync(id);
            if (entity != null)
            {
                _entities.Remove(entity);
            }
        }

        public async Task DeleteGuidAsync(Guid id)
        {
            var entity = await _entities.FindAsync(id);
            if (entity != null)
            {
                _entities.Remove(entity);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
