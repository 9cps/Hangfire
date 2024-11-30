using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities
{
    [Table("DATA_FTP_ALS")]
    [PrimaryKey(nameof(AsOfDate), nameof(AccountNo), nameof(Suffix), nameof(RefNo))]
    public class DataFtpAls
    {
        [Column(Order = 1)]
        [StringLength(10)]
        public string AccountNo { get; set; }   // varchar(10) NOT NULL

        [Column(Order = 2)]
        [StringLength(3)]
        public string Suffix { get; set; }      // varchar(3) NOT NULL

        [Column(Order = 3)]
        [StringLength(15)]
        public string RefNo { get; set; }       // varchar(15) NOT NULL, defaults to '   '

        [StringLength(7)]
        public string Product { get; set; }     // varchar(7) NULL

        [StringLength(12)]
        public string OldAgeM { get; set; }     // varchar(12) NULL

        [StringLength(12)]
        public string OldAgeD { get; set; }     // varchar(12) NULL

        [StringLength(100)]
        public string CustName { get; set; }    // varchar(100) NULL

        public DateTime? ContDate { get; set; } // datetime NULL
        public DateTime? MaturDate { get; set; } // datetime NULL
        public DateTime? FirstDue { get; set; } // datetime NULL
        public DateTime? SecDue { get; set; }   // datetime NULL
        public DateTime? ThirdDue { get; set; } // datetime NULL

        [StringLength(5)]
        public string ExitType { get; set; }    // varchar(5) NULL

        [StringLength(7)]
        public string TotDayPdue { get; set; }  // varchar(7) NULL

        [Column(Order = 4)]
        public DateTime AsOfDate { get; set; }  // datetime NOT NULL

        [StringLength(1)]
        public string FlagPost { get; set; }     // varchar(1) NULL
    }

}
