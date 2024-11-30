using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

// REMARK:  Batch result table
namespace Hangfire.Models.Entities
{
    [Table("TblResult")]
    [PrimaryKey(nameof(CustID), nameof(TDRSeqNo), nameof(YrMth), nameof(AccountNo), nameof(Suffix), nameof(ReferNo))]
    public class TblResult
    {
        [Column("CustID")]
        public string CustID { get; set; }

        [Column("TDRSeqNo")]
        public short TDRSeqNo { get; set; }

        [Column("YrMth")]
        public string YrMth { get; set; }

        [Column("ConditionPay")]
        public decimal ConditionPay { get; set; } = 0.0m;

        [Column("ActualPay")]
        public decimal ActualPay { get; set; } = 0.0m;

        [Column("PayLess")]
        public decimal PayLess { get; set; } = 0.0m;

        [Column("ResultCode")]
        public string ResultCode { get; set; } = string.Empty;

        [Column("MonthLess")]
        public short MonthLess { get; set; } = 0;

        [Column("MonthNPL")]
        public short MonthNPL { get; set; } = 0;

        [Column("MonthClass")]
        public int MonthClass { get; set; } = 0;

        [Column("Class")]
        public string Class { get; set; } = string.Empty;

        [Column("HoldFlag")]
        public bool HoldFlag { get; set; } = false;

        [Column("SuccessFlag")]
        public string SuccessFlag { get; set; }

        [Column("accountno")]
        public string AccountNo { get; set; }

        [Column("suffix")]
        public string Suffix { get; set; }

        [Column("referNo")]
        public string ReferNo { get; set; } = "   ";

        [Column("AccBFclose")]
        public string AccBFclose { get; set; }
    }
}