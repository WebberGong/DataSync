using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Attribute;
using Newtonsoft.Json;

namespace Entity
{
    [DefaultTable("YX_BASE_COAL")]
    public class Coal : BaseEntity
    {
        [Key]
        [Column("CID")]
        [JsonProperty("cid")]
        public string Id { get; set; }

        [Column("COAL_NAME")]
        [JsonIgnore]
        public string Name { get; set; }

        [Column("MODIFYTIME")]
        public override string UpdateTime { get; set; }

        public override string ComparerKey => Id;
    }
}
