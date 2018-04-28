using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_BASE_CUSTOMER")]
    public class Customer : BaseEntity
    {
        [Key]
        [Column("CUSTOMERID")]
        [JsonIgnore]
        public string Id { get; set; }

        [Column("CCODE")]
        [JsonProperty("ccode")]
        public string Code { get; set; }

        [Column("ZJM")]
        [JsonProperty("zjm")]
        public string Zjm { get; set; }

        [Column("JURISDICALPERSON")]
        [JsonProperty("juridicalPerson")]
        public string JuridicalPerson { get; set; }

        [Column("AREA")]
        [JsonProperty("area")]
        public string Area { get; set; }

        [Column("INDUSTRY")]
        [JsonProperty("industry")]
        public string Industry { get; set; }

        [Column("FAX")]
        [JsonProperty("fax")]
        public string Fax { get; set; }

        [Column("ADDRESS")]
        [JsonProperty("address")]
        public string Address { get; set; }

        [Column("ZIPCODE")]
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [Column("MEMDAY")]
        [JsonProperty("memDay")]
        public string MemorialDay { get; set; }

        [Column("ISVALID")]
        [JsonProperty("isvalid")]
        public int? IsValid { get; set; }

        [Column("BANK")]
        [JsonProperty("bank")]
        public string Bank { get; set; }

        [Column("ACCOUNT")]
        [JsonProperty("account")]
        public string Account { get; set; }

        [Column("MEMO")]
        [JsonProperty("memo")]
        public string Memo { get; set; }

        [Column("CUSTOMERLEVEL")]
        [JsonProperty("customerLevel")]
        public int? Level { get; set; }

        [Column("CNAME")]
        [JsonProperty("cname")]
        public string Name { get; set; }

        [Column("PHONE")]
        [JsonProperty("phone")]
        public string Phone { get; set; }

        [Column("CONTACTS")]
        [JsonProperty("contacts")]
        public string Contacts { get; set; }

        [Column("TAXNUMBER")]
        [JsonProperty("taxnumber")]
        public string TaxNumber { get; set; }

        [Column("MODIFYTIME")]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => TaxNumber;
    }
}