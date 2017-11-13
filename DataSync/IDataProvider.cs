using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;

namespace DataSync
{
    public interface IDataProvider<TEntity> where TEntity : BaseEntity
    {
        Task<IList<TEntity>> GetEntities();
    }
}