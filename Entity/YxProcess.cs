﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_YX_YXINFO")]
    public class YxProcess : BaseEntity
    {
        [Key]
        [Column("YXID")]
        [JsonIgnore]
        public string Id { get; set; }

        [NotMapped]
        [JsonProperty("dispatchSN")]
        public string DispatchSn => SendTruck?.DispatchSn;

        [Column("STID")]
        [JsonIgnore]
        public string SendTruckId { get; set; }

        [JsonIgnore]
        [ForeignKey("SendTruckId")]
        public SendTruck SendTruck { get; set; }

        [Column("WID")]
        [JsonIgnore]
        public string WeightId { get; set; }

        [JsonIgnore]
        [ForeignKey("WeightId")]
        public Weight Weight { get; set; }

        [Column("TID")]
        [JsonIgnore]
        public string TruckId { get; set; }

        [JsonIgnore]
        [ForeignKey("TruckId")]
        public Truck Truck { get; set; }

        [NotMapped]
        [JsonProperty("truck_no")]
        public string TruckNumber => Truck?.Number;

        [Column("YXSTATE")]
        [JsonProperty("yxStatus")]
        public int? YxStatus { get; set; }

        [NotMapped]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => Id;

        public new static Expression<Func<YxProcess, bool>> SynchronizationWhere()
        {
            return x => true;
        }
    }
}