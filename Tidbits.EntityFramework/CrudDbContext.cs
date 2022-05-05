using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tidbits.EntityFramework.Exceptions;

namespace Tidbits.EntityFramework
{
    public class CrudDbContext<TDerivingContext> : ReadOnlyDbContext<TDerivingContext>, ICrudDbContext where TDerivingContext : DbContext
    {
        public CrudDbContext(DbContextOptions<TDerivingContext> options) : base(options)
        {

        }

        public async Task<TEntity> AddNoTracking<TEntity>(TEntity entity) where TEntity : class
        {
            EntityEntry<TEntity>? trackedEntity = null;
            try
            {
                trackedEntity = await this.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
                await this.SaveChangesAsync().ConfigureAwait(false);
                var result = trackedEntity.Entity;
                return result;
            }
            catch (DbUpdateException e)
            {
                if (e!.InnerException!.Message!.Contains("FOREIGN KEY"))
                {
                    throw new DbForeignKeyException(e.Message, e);
                }
                throw;
            }
            finally
            {
                trackedEntity!.State = EntityState.Detached;
            }

        }

        public async Task<TEntity?> Remove<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class
        {
            var entity = await this.Set<TEntity>().SingleOrDefaultAsync(expression).ConfigureAwait(false);
            if (entity == null) return default;
            var result = this.Set<TEntity>().Remove(entity).Entity;
            await this.SaveChangesAsync().ConfigureAwait(false);
            return result;
        }

        public async Task<int> UpdateNoTracking<TEntity>(TEntity entity) where TEntity : class
        {
            return await UpdateNoTracking(
                entity,
                trackedEntity => trackedEntity.State = EntityState.Modified
                ).ConfigureAwait(false);
        }

        public async Task<int> UpdateNoTracking<TEntity>(TEntity entity, params string[] updatedPropertyNames) where TEntity : class
        {
            return await UpdateNoTracking(entity, trackedEntity =>
            {
                trackedEntity.State = EntityState.Unchanged;
                foreach (var updatedPropertyName in updatedPropertyNames)
                {
                    trackedEntity.Property(updatedPropertyName).IsModified = true;
                }
            }).ConfigureAwait(false);
        }

        public async Task<int> UpdateNoTracking<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertySelector) where TEntity : class
        {
            return await UpdateNoTracking(entity, trackedEntity =>
            {
                trackedEntity.State = EntityState.Unchanged;
                trackedEntity.Property(propertySelector).IsModified = true;
            }).ConfigureAwait(false);
        }
        public async Task<int> UpdateNoTracking<TEntity, TProperty, TProperty2>(
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertySelector,
            Expression<Func<TEntity, TProperty2>> propertySelector2
            ) where TEntity : class
        {
            return await UpdateNoTracking(entity, trackedEntity =>
            {
                trackedEntity.State = EntityState.Unchanged;
                trackedEntity.Property(propertySelector).IsModified = true;
                trackedEntity.Property(propertySelector2).IsModified = true;
            }).ConfigureAwait(false);
        }

        public async Task<int> UpdateNoTracking<TEntity, TProperty, TProperty2, TProperty3>(
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertySelector,
            Expression<Func<TEntity, TProperty2>> propertySelector2,
            Expression<Func<TEntity, TProperty2>> propertySelector3
        ) where TEntity : class
        {
            return await UpdateNoTracking(entity, trackedEntity =>
            {
                trackedEntity.State = EntityState.Unchanged;
                trackedEntity.Property(propertySelector).IsModified = true;
                trackedEntity.Property(propertySelector2).IsModified = true;
                trackedEntity.Property(propertySelector3).IsModified = true;
            }).ConfigureAwait(false);
        }

        private async Task<int> UpdateNoTracking<TEntity>(TEntity entity, Action<EntityEntry<TEntity>> modifiedPropertiesSetter) where TEntity : class
        {
            EntityEntry<TEntity>? trackedEntity = null;
            try
            {
                var currentVersion = Set<TEntity>().Local.SingleOrDefault(e => CompareKeys(e, entity));
                if (currentVersion != null)
                {
                    trackedEntity = Entry(currentVersion);
                    trackedEntity.State = EntityState.Detached;
                }

                trackedEntity = this.Set<TEntity>().Attach(entity);
                modifiedPropertiesSetter(trackedEntity);
                var affectedCount = await this.SaveChangesAsync().ConfigureAwait(false);
                return affectedCount;
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbNotFoundException("Entity not found in database", e);
            }
            catch (DbUpdateException e)
            {
                if (e!.InnerException!.Message!.Contains("FOREIGN KEY"))
                {
                    throw new DbForeignKeyException(e.Message, e);
                }

                throw;
            }
            finally
            {
                if (trackedEntity != null)
                    trackedEntity.State = EntityState.Detached;
            }
        }

        private bool CompareKeys<T>(T entity1, T entity2)
        {
            var keyProperties = Model
                !.FindEntityType(typeof(T))
                !.FindPrimaryKey()
                !.Properties
                .Select(p => p.PropertyInfo);

            foreach (var property in keyProperties)
            {
                var value1 = property!.GetValue(entity1);
                var value2 = property!.GetValue(entity2);
                
                if (!value1!.Equals(value2)) return false;
            }

            return true;
        }

    }
}