﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Entity;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;

namespace DataSync
{
    public class DataNotifier<TEntity> where TEntity : BaseEntity
    {
        private const string ResourceName = "ResourceName";

        private const string Info = "Info";

        private const string RowId = "Rowid";

        private const string QueryId = "QueryId";

        public static string QueryExpression => Assistance.GetQueryExpression<TEntity>();

        public static List<TEntity> CachedEntities => DataSynchronizer<TEntity>.SourceEntities;

        public static string EntityType => typeof(TEntity).FullName;

        public static async Task AddNotification(IDataProcesser<TEntity> dataProcesser,
            IDataProvider<TEntity> dataProvider)
        {
            try
            {
                var con = new OracleConnection(Settings.ConnectionString);
                var cmd = new OracleCommand(QueryExpression, con);
                var dep = new OracleDependency(cmd, false, 0, Settings.IsPersistentNotification)
                {
                    QueryBasedNotification = true,
                    RowidInfo = OracleRowidInfo.Include
                };
                dep.OnChange += async (src, args) =>
                {
                    using (var dbContext = new DatabaseContext<TEntity>())
                    {
                        foreach (DataRow row in args.Details.Rows)
                        {
                            var resourceName = (string)row[ResourceName];
                            var info = (OracleNotificationInfo)row[Info];
                            var rowId = (string)row[RowId];
                            var queryId = (long)row[QueryId];
                            var changedEntity = GetChangedEntity(dbContext, rowId, info);
                            if (changedEntity == null)
                                continue;
                            var changedMsg =
                                $"ResourceName: {resourceName}, Info: {info}, RowId: {rowId}, QueryId: {queryId}, Entity: {JsonConvert.SerializeObject(changedEntity)}";
                            await LogHelper.LogWarnAsync($"接收到数据修改通知: {changedMsg}");
                            if (dataProvider.SynchronizationWhere().Compile().Invoke(changedEntity))
                            {
                                if (await ProcessChangedData(dataProcesser, dataProvider, changedEntity, info))
                                    MaintainCachedEntities(changedEntity, info);
                            }
                            else
                            {
                                await LogHelper.LogInfoAsync(
                                    $"该数据状态不可用, 不需要同步: {changedMsg}");
                                MaintainCachedEntities(changedEntity, info);
                            }
                        }
                    }
                };

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                con.Close();
                cmd.Dispose();
                con.Dispose();
                await LogHelper.LogInfoAsync($"添加数据修改通知成功, 实体类型: {EntityType}");
            }
            catch (Exception ex)
            {
                Utility.LogFatalOrThrowException(ex, $"添加数据修改通知失败, 实体类型: {EntityType}");
            }
        }

        private static TEntity GetChangedEntity(DatabaseContext<TEntity> dbContext, string rowId,
            OracleNotificationInfo operationType)
        {
            ValidateOperationType(operationType);

            var entity = dbContext.Entities.IncludeAll().ToList().FirstOrDefault(x => x.RowId == rowId) ??
                CachedEntities.FirstOrDefault(x => x.RowId == rowId);
            return entity;
        }

        private static void MaintainCachedEntities(TEntity changedEntity, OracleNotificationInfo operationType)
        {
            ValidateOperationType(operationType);

            var cachedEntity = CachedEntities.FirstOrDefault(x => x.RowId == changedEntity.RowId);
            if (operationType != OracleNotificationInfo.Insert)
                CachedEntities.Remove(cachedEntity);
            if (operationType != OracleNotificationInfo.Delete)
                CachedEntities.Add(changedEntity);
        }

        private static void ValidateOperationType(OracleNotificationInfo operationType)
        {
            if (operationType != OracleNotificationInfo.Insert &&
                operationType != OracleNotificationInfo.Update &&
                operationType != OracleNotificationInfo.Delete)
                Utility.LogFatalOrThrowException(
                    new Exception($"未处理的数据修改通知类型: {operationType}, 实体类型: {EntityType}"));
        }

        private static async Task<bool> ProcessChangedData(IDataProcesser<TEntity> dataProcesser,
            IDataProvider<TEntity> dataProvider,
            TEntity changedEntity, OracleNotificationInfo operationType)
        {
            ValidateOperationType(operationType);

            var result = false;
            switch (operationType)
            {
                case OracleNotificationInfo.Insert:
                    result = await DataSynchronizer<TEntity>.Insert(dataProcesser, dataProvider, changedEntity);
                    await dataProcesser.Sync(changedEntity);
                    break;
                case OracleNotificationInfo.Update:
                    var cachedEntity = CachedEntities.FirstOrDefault(x => x.RowId == changedEntity.RowId);
                    result = await DataSynchronizer<TEntity>.Update(dataProcesser, dataProvider, changedEntity,
                        cachedEntity);
                    await dataProcesser.Sync(changedEntity);
                    break;
                case OracleNotificationInfo.Delete:
                    Utility.LogFatalOrThrowException(new Exception($"暂未处理删除数据操作, 实体类型: {EntityType}"));
                    result = false;
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }
    }
}