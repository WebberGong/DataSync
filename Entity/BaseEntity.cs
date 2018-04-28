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

        [NotMapped]
        [JsonIgnore]
        public virtual string ComparerValue => string.Empty;

        [JsonIgnore]
        [NotificationIgnore]
        public abstract string UpdateTime { get; set; }

        [NotMapped]
        [JsonIgnore]
        public bool Synchronized { get; set; }
    }
}