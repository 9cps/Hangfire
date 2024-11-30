using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities
{
    [Table("tblLogTransection")]
    public class TblLogTransection
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tran_date")]
        public DateTime TranDate { get; set; }

        [Column("strep_job")]
        [StringLength(100)]
        public string StrepJob { get; set; }

        [Column("status")]
        [StringLength(75)]
        public string? Status { get; set; }

        [Column("remark")]
        [StringLength(255)]
        public string Remark { get; set; }
    }
}
