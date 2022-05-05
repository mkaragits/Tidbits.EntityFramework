using System.Linq.Expressions;

namespace Tidbits.EntityFramework
{
    public interface IReadOnlyDbContext
    {
        /// <summary>
        /// Get single entity from database. Does not throw if entity does not exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <param name="expression">selector expression</param>
        /// <returns>entity or default</returns>
        Task<TEntity?> GetNoTracking<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;

        /// <summary>
        /// Get single entity projected according to parameters from database. Does not throw if entity does not exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity</typeparam>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="expression">selector expression</param>
        /// <param name="projection">projection expression</param>
        /// <returns>entity projected according to parameters or default</returns>
        Task<TResult?> GetNoTracking<TEntity, TResult>(
            Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, TResult>> projection) where TEntity : class;

        /// <summary>
        /// Get all entries from a table as IAsyncEnumerable.
        /// </summary>
        /// <typeparam name="TEntity">type of entity</typeparam>
        /// <returns>IAsyncEnumerable of entries in table</returns>
        IAsyncEnumerable<TEntity> ListAllNoTracking<TEntity>() where TEntity : class;

        /// <summary>
        /// Get all entries from a table projected to a new type as IAsyncEnumerable.
        /// </summary>
        /// <typeparam name="TEntity">type of entity</typeparam>
        /// <typeparam name="TResult">type of result of projection</typeparam>
        /// <param name="selector">projection expression</param>
        /// <returns>IAsyncEnumerable of entries in table projected to new type</returns>
        IAsyncEnumerable<TResult> ListAllNoTracking<TEntity, TResult>(Expression<Func<TEntity, TResult>> selector)
            where TEntity : class;

        /// <summary>
        /// Get entries matching expression from a table as IAsyncEnumerable.
        /// </summary>
        /// <typeparam name="TEntity">type of entity</typeparam>
        /// <param name="expression">selector expression</param>
        /// <returns>IAsyncEnumerable of entries in table</returns>
        IAsyncEnumerable<TEntity> ListNoTracking<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;

        /// <summary>
        /// Get entries matching expression from a table projected to a new type as IAsyncEnumerable.
        /// </summary>
        /// <typeparam name="TEntity">type of entity</typeparam>
        /// <typeparam name="TResult">type of result of projection</typeparam>
        /// <param name="expression">selector expression</param>
        /// <param name="projection">projection expression</param>
        /// <returns>IAsyncEnumerable of entries in table projected to new type</returns>
        IAsyncEnumerable<TResult> ListNoTracking<TEntity, TResult>(
            Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, TResult>> projection)
            where TEntity : class;


        /// <summary>
        /// Check if any entities exists in database matching the expression.
        /// </summary>
        /// <typeparam name="TEntity">type of entity</typeparam>
        /// <param name="expression">selector expression</param>
        /// <returns>true if a matching entity was found, false otherwise</returns>
        Task<bool> Exists<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;

    }
}
