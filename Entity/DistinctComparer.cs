using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class DistinctComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : BaseEntity
    {
        public bool Equals(TEntity x, TEntity y)
        {
            return x?.ComparerKey == y?.ComparerKey;
        }

        public int GetHashCode(TEntity obj)
        {
            return obj.ComparerKey.GetHashCode();
        }
    }
}
