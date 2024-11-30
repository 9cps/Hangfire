using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities
{
    [Table("TEMP_DWH_PDPA")]
    [PrimaryKey(nameof(AccountSuffix))]
    public class TempDwhPdpa
    {
        [Column("ACCOUNT_SUFFIX")]
        [MaxLength(17)]
        public string AccountSuffix { get; set; } = null!;

        [Column("CREATE_DATE")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("CONFIRM_DEL")]
        [MaxLength(5)]
        public string ConfirmDel { get; set; } = null!;

        [Column("CUSTOMER_STATUS")]
        [MaxLength(2)]
        public string ConfirmStatus { get; set; } = null!;

        [Column("UPDATE_DATE")]
        public DateTime? UpdateDate { get; set; }
    }

}
