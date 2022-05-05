using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Tidbits.EntityFramework
{
    public static class ContextExtensions
    {
        public static string GetTableName(this DbContext context, Type type)
        {
            var entityType = context.Model.FindEntityType(type);
            return entityType?.GetSchemaQualifiedTableName() ?? throw new ArgumentException("type is not an entity", nameof(type));
        }

        public static string GetTableName<TEntity>(this DbContext context)
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            return entityType?.GetSchemaQualifiedTableName() ?? throw new ArgumentException($"type {typeof(TEntity)} is not an entity");
        }

    }
}
