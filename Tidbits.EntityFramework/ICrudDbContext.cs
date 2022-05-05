using System.Linq.Expressions;

namespace Tidbits.EntityFramework
{
    internal interface ICrudDbContext
    {
        /// <summary>
        /// Add an entity to a table. Entity will be detached from change tracker immediately after.
        /// </summary>
        /// <typeparam name="TEntity">type of the entity</typeparam>
        /// <param name="entity">Entity to be added</param>
        /// <returns>The entity with identity fields filled</returns>
        Task<TEntity> AddNoTracking<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Remove and return a single entity from a table. Will not throw if entity is not found.
        /// </summary>
        /// <typeparam name="TEntity">type of the entity</typeparam>
        /// <param name="expression">entity selector expression</param>
        /// <returns>the deleted entity or default</returns>
        Task<TEntity?> Remove<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;

        /// <summary>
        /// Update an entity in the database. All fields will be marked modified.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">entity to be updated.</param>
        /// <returns>number of rows affected</returns>
        Task<int> UpdateNoTracking<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Update an entity in the database. Only the selected property will be marked modified.
        /// </summary>
        /// <typeparam name="TEntity">type of the entity</typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="entity">entity to be updated.</param>
        /// <param name="propertySelector"></param>
        /// <returns>number of rows affected</returns>
        Task<int> UpdateNoTracking<TEntity, TProperty>(TEntity entity,
            Expression<Func<TEntity, TProperty>> propertySelector) where TEntity : class;

        /// <summary>
        /// Update an entity in the database. Only properties selected in selectors will be marked modified.
        /// </summary>
        /// <typeparam name="TEntity">type of the entity</typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="TProperty2"></typeparam>
        /// <param name="entity">entity to be updated.</param>
        /// <param name="propertySelector"></param>
        /// <param name="propertySelector2"></param>
        /// <returns>number of rows affected</returns>
        Task<int> UpdateNoTracking<TEntity, TProperty, TProperty2>(
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertySelector,
            Expression<Func<TEntity, TProperty2>> propertySelector2
        ) where TEntity : class;

        /// <summary>
        /// Update an entity in the database. Only properties selected in selectors will be marked modified.
        /// </summary>
        /// <typeparam name="TEntity">type of the entity</typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="TProperty2"></typeparam>
        /// <typeparam name="TProperty3"></typeparam>
        /// <param name="entity">entity to be updated.</param>
        /// <param name="propertySelector"></param>
        /// <param name="propertySelector2"></param>
        /// <param name="propertySelector3"></param>
        /// <returns>number of rows affected</returns>
        Task<int> UpdateNoTracking<TEntity, TProperty, TProperty2, TProperty3>(
            TEntity entity,
            Expression<Func<TEntity, TProperty>> propertySelector,
            Expression<Func<TEntity, TProperty2>> propertySelector2,
            Expression<Func<TEntity, TProperty2>> propertySelector3
        ) where TEntity : class;

        /// <summary>
        /// Update an entity in the database. Only the properties named in the list will be marked modified.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">entity to be updated.</param>
        /// <param name="updatedPropertyNames"></param>
        /// <returns>number of rows affected</returns>
        Task<int> UpdateNoTracking<TEntity>(TEntity entity, params string[] updatedPropertyNames) where TEntity : class;
    }
}
