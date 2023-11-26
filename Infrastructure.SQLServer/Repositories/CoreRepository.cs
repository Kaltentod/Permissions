using Microsoft.EntityFrameworkCore;
using Permissions.Infrastructure.SQLServer.Repositories;
using System.Linq.Expressions;

namespace Permissions.Infrastructure.SQLServer
{
    public abstract class CoreRepository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        private readonly TContext context;
        public CoreRepository(TContext context)
        {
            this.context = context;
        }
        public async Task<TEntity> Create(TEntity entity)
        {
            await context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public async Task Delete(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
        }

        public async Task<TEntity> GetById(int id)
        {
            //return await context.Set<TEntity>().FindAsync(id);
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            var primaryKey = entityType.FindPrimaryKey().Properties.FirstOrDefault();
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var property = Expression.Property(parameter, primaryKey.Name);
            var equals = Expression.Equal(property, Expression.Constant(id));

            var lambda = Expression.Lambda<Func<TEntity, bool>>(equals, parameter);

            var query = context.Set<TEntity>().AsQueryable();
            query = IncludeAllNavigationProperties(query);
            return await query.FirstOrDefaultAsync(lambda);
        }

        public async Task<List<TEntity>> GetAll()
        {
            var query = context.Set<TEntity>().AsQueryable();
            query = IncludeAllNavigationProperties(query);
            return await query.ToListAsync();
        }

        public Task<TEntity> Update(TEntity entity)
        {
             context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        private IQueryable<TEntity> IncludeAllNavigationProperties(IQueryable<TEntity> query)
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            foreach (var navigation in entityType.GetNavigations())
            {
                query = query.Include(navigation.Name);
            }

            return query;
        }
    }
}