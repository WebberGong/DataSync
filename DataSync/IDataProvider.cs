using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using System.Linq.Expressions;
using System;

namespace DataSync
{
    public interface IDataProvider<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetEntities();

        List<TEntity> GetPeriodicalSyncEntities();

        bool IsUpdateOnly { get; }

        bool IsForceInitSync { get; }

        Expression<Func<TEntity, bool>> SynchronizationWhere();

        Expression<Func<TEntity, bool>> UpdateWhere(IEnumerable<TEntity> filter);

        Expression<Func<TEntity, bool>> DeleteWhere();
    }
}