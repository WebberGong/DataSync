using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_BASE_CARRIER")]
    public class Carrier : BaseEntity
    {
        [Key]
        [Column("CID")]
        [JsonIgnore]
        public string Id { get; set; }

        [Column("CARRIER_NAME")]
        [JsonProperty("carrierName")]
        public string Name { get; set; }

        [Column("PERSON")]
        [JsonProperty("person")]
        public string Person { get; set; }

        [Column("PERSON_TEL")]
        [JsonProperty("personTel")]
        public string PersonTel { get; set; }

        [Column("PHONE")]
        [JsonProperty("phone")]
        public string Phone { get; set; }

        [Column("ADDRESS")]
        [JsonProperty("address")]
        public string Address { get; set; }

        [Column("ISVALID")]
        [JsonProperty("isvalid")]
        public int? IsValid { get; set; }

        [Column("MEMO")]
        [JsonProperty("memo")]
        public string Memo { get; set; }

        [Column("TAXNUMBER")]
        [JsonProperty("taxnumber")]
        public string TaxNumber { get; set; }

        [Column("MODIFYTIME")]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => TaxNumber;

        public new static Expression<Func<Carrier, bool>> SynchronizationWhere()
        {
            return x => x.IsValid == 1;
        }
    }
}