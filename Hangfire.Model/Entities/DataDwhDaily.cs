using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities
{
    [Table("DATA_DWH_DAILY")]
    [PrimaryKey(nameof(AsOfDate), nameof(AccountNo), nameof(Suffix), nameof(RefNo))]
    public class DataDwhDaily
    {
        [Column(Order = 1)]
        public DateTime AsOfDate { get; set; } // datetime NOT NULL

        [Column(Order = 2)]
        [StringLength(12)]
        public string AccountNo { get; set; }   // varchar(12) NOT NULL

        [Column(Order = 3)]
        [StringLength(5)]
        public string Suffix { get; set; }      // varchar(5) NOT NULL

        [Column(Order = 4)]
        [StringLength(20)]
        public string RefNo { get; set; }       // varchar(20) NOT NULL

        [StringLength(20)]
        public string RefNoRm { get; set; }     // varchar(20) NULL

        public decimal? CurBookBal { get; set; } // numeric(18,3) NULL
        public decimal? OrigPaySchedule { get; set; } // numeric(18,3) NULL

        [StringLength(2)]
        public string DataSource { get; set; }  // varchar(2) NULL

        [StringLength(2)]
        public string Stage { get; set; }       // varchar(2) NULL

        public bool? GeneralPdFlag { get; set; } // bit NULL
        public bool? SeverePdFlag { get; set; } // bit NULL
    }

}
