using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_BASE_TRUCK")]
    public class Truck : BaseEntity
    {
        [Key]
        [Column("TID")]
        [JsonIgnore]
        public string Id { get; set; }

        [Column("TRUCK_NO")]
        [JsonProperty("truck_no")]
        public string Number { get; set; }

        [Column("TRUCK_TYPE")]
        [JsonProperty("truck_type")]
        public string Type { get; set; }

        [Column("TRUCK_COLOUR")]
        [JsonProperty("truck_colour")]
        public string Color { get; set; }

        [Column("TRUCK_LENGTH")]
        [JsonProperty("truck_length")]
        public double? Length { get; set; }

        [Column("TRUCK_WIDTH")]
        [JsonProperty("truck_width")]
        public double? Width { get; set; }

        [Column("OWNER")]
        [JsonProperty("owner")]
        public string Owner { get; set; }

        [Column("OWNER_TEL")]
        [JsonProperty("owner_tel")]
        public string OwnerTelephoneNumber { get; set; }

        [Column("OWNER_ADD")]
        [JsonProperty("owner_add")]
        public string OwnerAddress { get; set; }

        [Column("DRIVER")]
        [JsonProperty("driver")]
        public string Driver { get; set; }

        [Column("DRIVER_TEL")]
        [JsonProperty("driver_tel")]
        public string DriverTelephoneNumber { get; set; }

        [Column("ISVALID")]
        [JsonProperty("isvalid")]
        public int? IsValid { get; set; }

        [Column("LOADING_TOTAL")]
        [JsonProperty("loading_total")]
        public double? LoadingTotal { get; set; }

        [Column("MEMO")]
        [JsonProperty("memo")]
        public string Memo { get; set; }

        [Column("MODIFYTIME")]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => Number;

        public new static Expression<Func<Truck, bool>> SynchronizationWhere()
        {
            return x => x.IsValid == 1;
        }
    }
}