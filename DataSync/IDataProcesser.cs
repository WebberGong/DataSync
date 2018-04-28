using System.Threading.Tasks;
using Entity;

namespace DataSync
{
    public interface IDataProcesser<in TEntity> where TEntity : BaseEntity
    {
        Task<bool> Save(TEntity entity);

        Task Sync(TEntity entity);
    }
}