using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Reflection;
using Common;
using Entity;
using Entity.Attribute;
using Newtonsoft.Json;
using RestSharp;

namespace DataSync
{
    public class Assistance
    {
        private const string Select = "select";
        private const string From = "from";
        private const string And = "and";

        private static readonly ConcurrentDictionary<string, IList<PropertyInfo>> PropsDictionary =
            new ConcurrentDictionary<string, IList<PropertyInfo>>();

        public static IList<PropertyInfo> GetPropertiesHaveSpecifiedAttribute<TEntity, TAttribute>()
            where TEntity : BaseEntity
            where TAttribute : Attribute
        {
            var entityType = typeof(TEntity);
            var attributeType = typeof(TAttribute);
            var key = $"{entityType.FullName},{attributeType.FullName}";
            IList<PropertyInfo> value;
            if (PropsDictionary.TryGetValue(key, out value)) return value;
            var props = GetProperties<TEntity>();
            var results =
                (from prop in props let attr = prop.GetCustomAttribute<TAttribute>() where attr != null select prop)
                .ToList();
            PropsDictionary.TryAdd(key, results);
            return results;
        }

        public static IList<PropertyInfo> GetProperties<TEntity>()
            where TEntity : BaseEntity
        {
            var entityType = typeof(TEntity);
            var key = $"{entityType.FullName}";
            IList<PropertyInfo> value;
            if (PropsDictionary.TryGetValue(key, out value)) return value;
            var props = entityType.GetProperties();
            PropsDictionary.TryAdd(key, props);
            return props;
        }

        public static PropertyInfo GetPropertie<TEntity>(string name)
            where TEntity : BaseEntity
        {
            var props = GetProperties<TEntity>();
            return props.FirstOrDefault(x => x.Name == name);
        }

        public static string GetTableName<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity);
            var attr = type.GetCustomAttribute<DefaultTableAttribute>();
            if (attr != null)
                return string.IsNullOrEmpty(attr.Schema) ? $"{attr.Name}" : $"{attr.Schema}.{attr.Name}";
            Utility.LogFatalOrThrowException(new Exception($"代码错误, 实体类声明部分必须包含DefaultTableAttribute特性, 类名: {type.FullName}"));
            return null;
        }

        public static string GetColumnsName<TEntity>() where TEntity : BaseEntity
        {
            var props = GetProperties<TEntity>();
            var columnsName = (from prop in props
                let colAttr = prop.GetCustomAttribute<ColumnAttribute>()
                let ignoreAttr = prop.GetCustomAttribute<NotificationIgnoreAttribute>()
                where colAttr != null && ignoreAttr == null
                select colAttr.Name).ToList();
            return string.Join(",", columnsName);
        }

        public static string GetWhereExpression<TEntity>() where TEntity : BaseEntity
        {
            var props = GetProperties<TEntity>();
            var whereConditions = new List<string>();
            foreach (var prop in props)
            {
                var propConditions = new List<string>();
                var colAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (colAttr != null)
                {
                    var whereAttrs = prop.GetCustomAttributes<NotificationWhereAttribute>();
                    foreach (var attr in whereAttrs)
                        if (!string.IsNullOrEmpty(attr.Condition))
                            propConditions.Add($"{colAttr.Name}{attr.Condition}");
                        else
                            Utility.LogFatalOrThrowException(
                                new Exception("代码错误, WhereAttribute特性中的Condition属性不能设置为null或者空字符串"));
                }
                if (propConditions.Count > 1)
                    whereConditions.Add($"({string.Join($" {And} ", propConditions)})");
                else if (propConditions.Count > 0)
                    whereConditions.Add(string.Join($" {And} ", propConditions));
            }
            var whereExpression = string.Join($" {And} ", whereConditions);
            if (!string.IsNullOrEmpty(whereExpression))
                whereExpression = $"where {whereExpression}";
            return whereExpression;
        }

        public static string GetQueryExpression<TEntity>() where TEntity : BaseEntity
        {
            return
                $"{Select} {GetColumnsName<TEntity>()} {From} {GetTableName<TEntity>()} {GetWhereExpression<TEntity>()}";
        }
    }
}