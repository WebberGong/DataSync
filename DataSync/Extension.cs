using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Entity;

namespace DataSync
{
    public static class Extension
    {
        public static IQueryable<TEntity> IncludeAll<TEntity>(this IQueryable<TEntity> entities)
            where TEntity : BaseEntity
        {
            var foreignKeyProps = Utility.GetPropertiesHaveSpecifiedAttribute<TEntity, ForeignKeyAttribute>();
            return foreignKeyProps.Aggregate(entities, (current, prop) => current.Include(prop.Name));
        }

        public static bool EqualsTo<TEntity>(this TEntity a, TEntity b) where TEntity : BaseEntity
        {
            var props = Utility.GetPropertiesHaveSpecifiedAttribute<TEntity, ColumnAttribute>();
            var result = true;
            foreach (var prop in props)
                if (prop.PropertyType.IsValueType && prop.GetValue(a) != prop.GetValue(b))
                {
                    result = false;
                    break;
                }
            return result;
        }

        public static void AssignTo<TEntity>(this TEntity a, TEntity b) where TEntity : BaseEntity
        {
            if (a == null || b == null)
                return;
            var props = Utility.GetProperties<TEntity>();
            foreach (var prop in props)
                if (prop.SetMethod != null)
                    prop.SetValue(b, prop.GetValue(a));
        }
    }
}