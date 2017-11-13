using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Common;
using Entity;
using LinqKit;
using Newtonsoft.Json;

namespace DataSync
{
    public class DataSynchronizer<TEntity> where TEntity : BaseEntity
    {
        private const string SynchronizationWhere = "SynchronizationWhere";

        private static readonly ConcurrentDictionary<string, Expression<Func<TEntity, bool>>>
            SynchronizationWhereDictionary =
                new ConcurrentDictionary<string, Expression<Func<TEntity, bool>>>();

        public static string EntityType => typeof(TEntity).FullName;

        public static IList<TEntity> SourceEntities { get; private set; }

        public static IList<TEntity> DestinationEntities { get; private set; }

        public static IList<TEntity> SyncEntities { get; private set; }

        public static Expression<Func<TEntity, bool>> GetSynchronizationWhere()
        {
            var entityType = typeof(TEntity);
            var key = $"{entityType.FullName}";
            Expression<Func<TEntity, bool>> value;
            if (SynchronizationWhereDictionary.TryGetValue(key, out value)) return value;
            var method = entityType.GetMethod(SynchronizationWhere);
            var expression = (Expression<Func<TEntity, bool>>) method.Invoke(null, null);
            SynchronizationWhereDictionary.TryAdd(key, expression);
            return expression;
        }

        public static async Task InitData(IDataProvider<TEntity> dataProvider)
        {
            try
            {
                using (var dbContext = new DatabaseContext<TEntity>())
                {
                    var predicate = PredicateBuilder.New<TEntity>(true);
                    predicate = predicate.And(GetSynchronizationWhere());

                    if (!Settings.IsSyncAllData && HaveUpdateTimeColumn())
                    {
                        var lastSyncTime = Settings.GetLastSyncTime();
                        predicate = predicate.And(x =>
                            string.Compare(x.UpdateTime, lastSyncTime, StringComparison.Ordinal) > 0 ||
                            string.IsNullOrEmpty(x.UpdateTime));
                    }
                    SourceEntities = await dbContext.Entities.IncludeAll().ToListAsync();
                    SyncEntities = SourceEntities.Where(predicate).ToList();
                    DestinationEntities = await dataProvider.GetEntities();
                }
            }
            catch (Exception ex)
            {
                Utility.LogFatalOrThrowException(ex, $"初始化数据异常, 实体类型: {EntityType}");
            }
        }

        public static async Task Synchronize(IDataProcesser<TEntity> dataProcesser, IDataProvider<TEntity> dataProvider)
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var sourceEntity in SyncEntities)
                    if (DestinationEntities.Any(x => x.ComparerKey == sourceEntity.ComparerKey))
                    {
                        var destinationEntities =
                            DestinationEntities.Where(x => x.ComparerKey == sourceEntity.ComparerKey);
                        foreach (var destinationEntity in destinationEntities)
                            tasks.Add(Update(dataProcesser, dataProvider, sourceEntity, destinationEntity));
                    }
                    else
                    {
                        tasks.Add(Insert(dataProcesser, dataProvider, sourceEntity));
                    }
                Task.WaitAll(tasks.ToArray());
                if (Settings.IsUpdateDateTime && HaveUpdateTimeColumn())
                    await UpdateDateTime();
            }
            catch (Exception ex)
            {
                Utility.LogFatalOrThrowException(ex, $"同步数据异常, 实体类型: {EntityType}");
            }
        }

        public static async Task<bool> Insert(IDataProcesser<TEntity> dataProcesser,
            IDataProvider<TEntity> dataProvider, TEntity sourceEntity)
        {
            var msg =
                $"添加数据: {JsonConvert.SerializeObject(sourceEntity)}";

            try
            {
                if (await dataProcesser.Save(sourceEntity))
                {
                    sourceEntity.Synchronized = true;
                    DestinationEntities.Add(sourceEntity);
                    await LogHelper.LogInfoAsync($"{msg}, 成功, 实体类型: {EntityType}");
                    return true;
                }
                await LogHelper.LogErrorAsync($"{msg}, 失败, 实体类型: {EntityType}");
                return false;
            }
            catch (Exception ex)
            {
                Utility.LogFatalOrThrowException(ex, $"{msg}, 异常, 实体类型: {EntityType}");
                return false;
            }
        }

        public static async Task<bool> Update(IDataProcesser<TEntity> dataProcesser,
            IDataProvider<TEntity> dataProvider, TEntity sourceEntity,
            TEntity destinationEntity)
        {
            var msg =
                $"修改数据: {JsonConvert.SerializeObject(sourceEntity)}";

            try
            {
                if (await dataProcesser.Save(sourceEntity))
                {
                    sourceEntity.Synchronized = true;
                    sourceEntity.AssignTo(destinationEntity);
                    await LogHelper.LogInfoAsync($"{msg}, 成功, 实体类型: {EntityType}");
                    return true;
                }
                await LogHelper.LogErrorAsync($"{msg}, 失败, 实体类型: {EntityType}");
                return false;
            }
            catch (Exception ex)
            {
                Utility.LogFatalOrThrowException(ex, $"{msg}, 异常, 实体类型: {EntityType}");
                return false;
            }
        }

        private static async Task UpdateDateTime()
        {
            try
            {
                using (var dbContext = new DatabaseContext<TEntity>())
                {
                    var synchronizedEntities = SyncEntities.Where(x => x.Synchronized).ToList();
                    var now = DateTime.Now.ToFormattedString();
                    foreach (var item in synchronizedEntities)
                    {
                        item.UpdateTime = now;
                        var updateItem = await dbContext.Entities.FirstOrDefaultAsync(x => x.RowId == item.RowId);
                        if (updateItem != null)
                            updateItem.UpdateTime = now;
                    }
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Utility.LogFatalOrThrowException(ex, $"修改更新时间异常, 实体类型: {EntityType}");
            }
        }

        private static bool HaveUpdateTimeColumn()
        {
            var prop = Utility.GetPropertie<TEntity>("UpdateTime");
            return prop?.GetCustomAttribute<ColumnAttribute>() != null;
        }
    }
}