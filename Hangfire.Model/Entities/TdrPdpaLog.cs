using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities
{
    [Table("TDR_PDPA_DEL_LOG")]
    public class TdrPdpaDelLog
    {
        [Key]
        [Column("ACCOUNT_SUFFIX")]
        [StringLength(17)]
        public string? AccountSuffix { get; set; }

        //[Column("SUFFIX_NO")]
        //[StringLength(5)]
        //public string? SuffixNo { get; set; }

        //[Column("REF_NO")]
        //[StringLength(20)]
        //public string? RefNo { get; set; }

        [Column("DEL_DATE")]
        public DateTime DelDate { get; set; }

        [Column("CUST_NAME")]
        [StringLength(200)]
        public string? CustName { get; set; }
    }
}
