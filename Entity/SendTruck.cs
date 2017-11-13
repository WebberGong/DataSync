using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_YS_SENDTRUCK")]
    public class SendTruck : BaseEntity
    {
        [Key]
        [Column("STID")]
        [JsonIgnore]
        public string Id { get; set; }

        [Column("SENDTRUCK_CODE")]
        [JsonIgnore]
        public string DispatchSn { get; set; }

        [NotMapped]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => Id;

        public new static Expression<Func<SendTruck, bool>> SynchronizationWhere()
        {
            return x => true;
        }
    }
}