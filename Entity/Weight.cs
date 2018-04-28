using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_YS_WEIGHT")]
    public class Weight : BaseEntity
    {
        [Key]
        [Column("WID")]
        [JsonIgnore]
        public string Id { get; set; }

        [NotMapped]
        [JsonProperty("dispatchSN")]
        public string DispatchSn { get; set; }

        [Column("TRUCK_NO")]
        [JsonProperty("truck_no")]
        public string TruckNumber { get; set; }

        [Column("PZ")]
        [JsonProperty("pz")]
        public double? Pz { get; set; }

        [Column("MZ")]
        [JsonProperty("mz")]
        public double? Mz { get; set; }

        [Column("JZ")]
        [JsonProperty("jz")]
        public double? Jz { get; set; }

        [NotMapped]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => DispatchSn;
    }
}