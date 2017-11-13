using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    public abstract class BaseEntity
    {
        [Column("ROWID")]
        [JsonIgnore]
        public string RowId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public abstract string ComparerKey { get; }

        [JsonIgnore]
        [NotificationIgnore]
        public abstract string UpdateTime { get; set; }

        [NotMapped]
        [JsonIgnore]
        public bool Synchronized { get; set; }

        public static Expression<Func<BaseEntity, bool>> SynchronizationWhere()
        {
            return x => true;
        }
    }
}