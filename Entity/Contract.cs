using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_HT_CONTRACT")]
    public class Contract : BaseEntity
    {
        [Key]
        [Column("CID")]
        [JsonProperty("cid")]
        public string Id { get; set; }

        [Column("CUSTOMERID")]
        [JsonIgnore]
        public string CustomerId { get; set; }

        [JsonIgnore]
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [NotMapped]
        [JsonProperty("customerTaxnumber")]
        public string CustomerTaxNumber => Customer?.TaxNumber;

        [Column("CONTRACTCODE")]
        [JsonProperty("contractCode")]
        public string Code { get; set; }

        [Column("CONTRACTNAME")]
        [JsonProperty("contractName")]
        public string Name { get; set; }

        [Column("SELLERID")]
        [JsonIgnore]
        public string SellerId { get; set; }

        [JsonIgnore]
        [ForeignKey("SellerId")]
        public Seller Seller { get; set; }

        [NotMapped]
        [JsonProperty("sellerTaxnumber")]
        public string SellerTaxNumber => Seller?.TaxNumber;

        [Column("CONTRACTTYPE")]
        [JsonProperty("contractType")]
        public int? Type { get; set; }

        [Column("COALID")]
        [JsonProperty("coalID")]
        public string CoalId { get; set; }

        [Column("AREA")]
        [JsonProperty("area")]
        public string Area { get; set; }

        [Column("QUALITY")]
        [JsonProperty("quality")]
        public string Quality { get; set; }

        [Column("UNIT")]
        [JsonProperty("unit")]
        public string Unit { get; set; }

        [Column("TOTALNUM")]
        [JsonProperty("totalNum")]
        public double? TotalNum { get; set; }

        [Column("SALENUM")]
        [JsonProperty("saleNum")]
        public double? SaleNum { get; set; }

        [Column("EXECNUMBER")]
        [JsonProperty("execNumber")]
        public int? ExecNumber { get; set; }

        [Column("STARTDATE")]
        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        [Column("ENDDATE")]
        [JsonProperty("endDate")]
        public string EndDate { get; set; }

        [Column("SIGNDATE")]
        [JsonProperty("signDate")]
        public string SignDate { get; set; }

        [Column("SIGNADDRESS")]
        [JsonProperty("signAddress")]
        public string SignAddress { get; set; }

        [Column("SIGNUSER")]
        [JsonProperty("signUser")]
        public string SignUser { get; set; }

        [Column("TRANSPORTTYPE")]
        [JsonProperty("transportType")]
        public string TransportType { get; set; }

        [Column("SETOUT")]
        [JsonProperty("setOut")]
        public string SetOut { get; set; }

        [Column("ARRIVE")]
        [JsonProperty("arrive")]
        public string Arrive { get; set; }

        [Column("LOWERLIMIT")]
        [JsonProperty("lowerLimit")]
        public double? LowerLimit { get; set; }

        [Column("CREDAMOUNT")]
        [JsonProperty("credAmount")]
        public double? CredAmount { get; set; }

        [Column("REPAYMENTPLANDATE")]
        [JsonProperty("repaymentPlanDate")]
        public string RepaymentPlanDate { get; set; }

        [Column("ISSUPPLEMENT")]
        [JsonProperty("isSupplement")]
        public string IsSupplement { get; set; }

        [Column("ISCHANGE")]
        [JsonProperty("isChange")]
        public string IsChange { get; set; }

        [Column("SPSTATE")]
        [JsonProperty("spState")]
        public int? SpState { get; set; }

        [Column("CONTRACTSTATE")]
        [JsonProperty("contractState")]
        public int? ContractState { get; set; }

        [Column("MEMO")]
        [JsonProperty("memo")]
        public string Memo { get; set; }

        [Column("MODIFYTIME")]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => Code;

        public new static Expression<Func<Contract, bool>> SynchronizationWhere()
        {
            return x => x.SpState == 2;
        }
    }
}