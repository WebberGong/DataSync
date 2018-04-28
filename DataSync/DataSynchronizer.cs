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
        public static string EntityType => typeof(TEntity).FullName;

        public static List<TEntity> SourceEntities { get; private set; }

        public static List<TEntity> DestinationEntities { get; private set; }

        public static List<TEntity> SyncFaildEntities { get; private set; }

        public static async Task InitData(IDataProvider<TEntity> dataProvider)
        {
            try
            {
                using (var dbContext = new DatabaseContext<TEntity>())
                {
                    var predicate = PredicateBuilder.New<TEntity>(true);
                    predicate = predicate.And(dataProvider.SynchronizationWhere());

                    if (!Settings.IsSyncAllData && HaveUpdateTimeColumn())
                    {
                        var lastSyncTime = Settings.GetLastSyncTime();
                        predicate = predicate.And(x =>
                            string.Compare(x.UpdateTime, lastSyncTime, StringComparison.CurrentCulture) >= 0 ||
                            string.IsNullOrEmpty(x.UpdateTime));
                    }
                    DestinationEntities = await dataProvider.GetEntities();
                    if (dataProvider.IsUpdateOnly)
                    {
                        predicate = predicate.And(dataProvider.UpdateWhere(DestinationEntities));
                    }
                    SourceEntities = await dbContext.Entities.IncludeAll().Where(predicate).ToListAsync();
                    SyncFaildEntities = new List<TEntity>();
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
                foreach (var sourceEntity in SourceEntities)
                    if (DestinationEntities.Any(x => x.ComparerKey == sourceEntity.ComparerKey))
                    {
                        if (!Settings.IsSyncNewDataOnly)
                        {
                            var destinationEntities =
                                DestinationEntities.Where(x => x.ComparerKey == sourceEntity.ComparerKey);
                            foreach (var destinationEntity in destinationEntities)
                                if (dataProvider.IsForceInitSync || sourceEntity.ComparerValue != destinationEntity.ComparerValue)
                                    tasks.Add(Update(dataProcesser, dataProvider, sourceEntity, destinationEntity));
                        }
                    }
                    else
                    {
                        if (!dataProvider.IsUpdateOnly)
                            tasks.Add(Insert(dataProcesser, dataProvider, sourceEntity));
                    }
                await Task.WhenAll(tasks.ToArray());
                if (Settings.IsUpdateDateTime && HaveUpdateTimeColumn())
                    await UpdateDateTime();

                var keys = DestinationEntities.Where(dataProvider.DeleteWhere().Compile()).Select(x => x.ComparerKey).ToList();
                DestinationEntities.RemoveAll(x => keys.Contains(x.ComparerKey));
                SourceEntities.RemoveAll(x => keys.Contains(x.ComparerKey));
            }
            catch (Exception ex)
            {
                Utility.LogFatalOrThrowException(ex, $"同步数据异常, 实体类型: {EntityType}");
            }
        }

        public static async Task PeriodicalSynchronize(IDataProcesser<TEntity> dataProcesser, IDataProvider<TEntity> dataProvider)
        {
            var entities = dataProvider.GetPeriodicalSyncEntities();
            var syncCount = entities.Count;
            var faildCount = SyncFaildEntities.Count;
            await LogHelper.LogInfoAsync($"定时同步, 实体类型: {EntityType}, 需要同步的数据总数: {syncCount}, 同步失败的数据总数: {faildCount}");
            var comparerKeys = entities.Select(x => x.ComparerKey).ToList();
            SyncFaildEntities = SyncFaildEntities.Where(x => !comparerKeys.Contains(x.ComparerKey)).ToList();
            entities.AddRange(SyncFaildEntities);
            foreach (var entity in entities)
            {
                var existed = DestinationEntities.FirstOrDefault(x => x.ComparerKey == entity.ComparerKey);
                if (entity.ComparerKey == null || existed == null)
                {
                    if (await dataProcesser.Save(entity))
                    {
                        await dataProcesser.Sync(entity);
                        entity.Synchronized = true;
                        DestinationEntities.Add(entity);
                        if (SyncFaildEntities.Any(x => x.ComparerKey == entity.ComparerKey))
                            SyncFaildEntities.Remove(entity);
                    }
                    else
                    {
                        if (SyncFaildEntities.All(x => x.ComparerKey != entity.ComparerKey || x.ComparerValue != entity.ComparerValue))
                            SyncFaildEntities.Add(entity);
                    }
                }
                else
                {
                    if (entity.ComparerValue != existed.ComparerValue)
                    {
                        if (await dataProcesser.Save(entity))
                        {
                            await dataProcesser.Sync(entity);
                            entity.Synchronized = true;
                            entity.AssignTo(existed);
                            if (SyncFaildEntities.Any(x => x.ComparerKey == entity.ComparerKey))
                                SyncFaildEntities.Remove(entity);
                        }
                        else
                        {
                            if (SyncFaildEntities.All(x => x.ComparerKey != entity.ComparerKey || x.ComparerValue != entity.ComparerValue))
                                SyncFaildEntities.Add(entity);
                        }
                    }
                }
            }
            var keys = DestinationEntities.Where(dataProvider.DeleteWhere().Compile()).Select(x => x.ComparerKey).ToList();
            DestinationEntities.RemoveAll(x => keys.Contains(x.ComparerKey));
            SourceEntities.RemoveAll(x => keys.Contains(x.ComparerKey));
        }

        public static async Task<bool> Insert(IDataProcesser<TEntity> dataProcesser,
            IDataProvider<TEntity> dataProvider, TEntity sourceEntity)
        {
            var msg =
                $"添加数据: {JsonConvert.SerializeObject(sourceEntity)}";

            try
            {
                if (await RetryWhenFaild(dataProcesser.Save(sourceEntity), 3))
                {
                    sourceEntity.Synchronized = true;
                    DestinationEntities.Add(sourceEntity);
                    if (SyncFaildEntities.Any(x => x.ComparerKey == sourceEntity.ComparerKey))
                        SyncFaildEntities.Remove(sourceEntity);
                    await LogHelper.LogInfoAsync($"{msg}, 成功, 实体类型: {EntityType}");
                    return true;
                }
                if (SyncFaildEntities.All(x => x.ComparerKey != sourceEntity.ComparerKey || x.ComparerValue != sourceEntity.ComparerValue))
                    SyncFaildEntities.Add(sourceEntity);
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
                if (await RetryWhenFaild(dataProcesser.Save(sourceEntity), 3))
                {
                    sourceEntity.Synchronized = true;
                    sourceEntity.AssignTo(destinationEntity);
                    if (SyncFaildEntities.Any(x => x.ComparerKey == sourceEntity.ComparerKey))
                        SyncFaildEntities.Remove(sourceEntity);
                    await LogHelper.LogInfoAsync($"{msg}, 成功, 实体类型: {EntityType}");
                    return true;
                }
                if (SyncFaildEntities.All(x => x.ComparerKey != sourceEntity.ComparerKey || x.ComparerValue != sourceEntity.ComparerValue))
                    SyncFaildEntities.Add(sourceEntity);
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
                    var synchronizedEntities = SourceEntities.Where(x => x.Synchronized).ToList();
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

        public static bool HaveUpdateTimeColumn()
        {
            var prop = Assistance.GetPropertie<TEntity>("UpdateTime");
            return prop?.GetCustomAttribute<ColumnAttribute>() != null;
        }

        private static async Task<bool> RetryWhenFaild(Task<bool> task, int maxRetryCount)
        {
            int retryCount = 0;
            bool result;
            do
            {
                result = await task;
                retryCount++;
            }
            while (!result && retryCount < maxRetryCount);
            return result;
        }
    }
}