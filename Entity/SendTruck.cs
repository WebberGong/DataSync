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

        [Column("ST_TIME")]
        [JsonIgnore]
        public string SendTruckTime { get; set; }

        [Column("CARRIERID")]
        [JsonIgnore]
        public string CarrierId { get; set; }

        [JsonIgnore]
        [ForeignKey("CarrierId")]
        public Carrier Carrier { get; set; }

        [Column("CONTRACTID")]
        [JsonIgnore]
        public string ContractId { get; set; }

        [JsonIgnore]
        [ForeignKey("ContractId")]
        public Contract Contract { get; set; }

        [NotMapped]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => Id;
    }
}