using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Tidbits.EntityFramework
{
    public class ReadOnlyDbContext<TDerivingContext> : DbContext, IReadOnlyDbContext where TDerivingContext : DbContext 
    {
        public ReadOnlyDbContext(DbContextOptions<TDerivingContext> options) : base(options) { }

        public async Task<TEntity?> GetNoTracking<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            return await this
                .Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(expression)
                .ConfigureAwait(false);
        }

        public async Task<TResult?> GetNoTracking<TEntity, TResult>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TResult>> projection) where TEntity : class
        {
            return await this
                .Set<TEntity>()
                .AsNoTracking()
                .Where(expression)
                .Select(projection)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async IAsyncEnumerable<TEntity> ListAllNoTracking<TEntity>() where TEntity : class
        {
            await foreach (var entity in this.Set<TEntity>()
                               .AsNoTracking()
                               .AsAsyncEnumerable()
                               .ConfigureAwait(false))
            {
                yield return entity;
            }
        }
        public async IAsyncEnumerable<TResult> ListAllNoTracking<TEntity, TResult>(Expression<Func<TEntity,TResult>> selector) where TEntity : class
        {
            await foreach (var entity in this.Set<TEntity>()
                               .AsNoTracking()
                               .Select(selector)
                               .AsAsyncEnumerable()
                               .ConfigureAwait(false))
            {
                yield return entity;
            }
        }

        public async IAsyncEnumerable<TEntity> ListNoTracking<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            await foreach (var entity in this.Set<TEntity>()
                               .AsNoTracking()
                               .Where(expression)
                               .AsAsyncEnumerable()
                               .ConfigureAwait(false))
            {
                yield return entity;
            }
        }

        public async IAsyncEnumerable<TResult> ListNoTracking<TEntity, TResult>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TResult>> projection) where TEntity : class
        {
            await foreach (var entity in this.Set<TEntity>()
                               .AsNoTracking()
                               .Where(expression)
                               .Select(projection)
                               .AsAsyncEnumerable()
                               .ConfigureAwait(false))
            {
                yield return entity;
            }
        }


        public async Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            return await this
                .Set<TEntity>()
                .AnyAsync(expression)
                .ConfigureAwait(false);
        }

    }
}
