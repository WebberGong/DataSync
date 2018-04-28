using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_BASE_SELLER")]
    public class Seller : BaseEntity
    {
        [Key]
        [Column("SID")]
        [JsonIgnore]
        public string Id { get; set; }

        [Column("SCODE")]
        [JsonProperty("scode")]
        public string Code { get; set; }

        [Column("SNAME")]
        [JsonProperty("sname")]
        public string Name { get; set; }

        [Column("COMPANY")]
        [JsonProperty("company")]
        public string Company { get; set; }

        [Column("JURISDICALPERSON")]
        [JsonProperty("juridicalPerson")]
        public string JuridicalPerson { get; set; }

        [Column("PROXY")]
        [JsonProperty("proxy")]
        public string Proxy { get; set; }

        [Column("TAXPAYER")]
        [JsonProperty("taxPayer")]
        public string TaxPayer { get; set; }

        [Column("PHONE")]
        [JsonProperty("phone")]
        public string Phone { get; set; }

        [Column("ADDRESS")]
        [JsonProperty("address")]
        public string Address { get; set; }

        [Column("ZIPCODE")]
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [Column("BANK")]
        [JsonProperty("bank")]
        public string Bank { get; set; }

        [Column("ACCOUNT")]
        [JsonProperty("account")]
        public string Account { get; set; }

        [Column("COUNTCOMPANY")]
        [JsonProperty("countcompany")]
        public string CountCompany { get; set; }

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
    }
}