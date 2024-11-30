using System.ComponentModel.DataAnnotations.Schema;

// REMARK:  Batch result table
namespace Hangfire.Models.Entities
{
    [Table("Loc_zCustIDLast")]
    public class LocZCustIDLast
    {
        [Column("Custid")]
        public string CustId { get; set; }

        [Column("Accno")]
        public string AccNo { get; set; }

        [Column("Suffix")]
        public string Suffix { get; set; }

        [Column("TDRSeqno")]
        public int? TDRSeqNo { get; set; }

        [Column("ReferNo")]
        public string ReferNo { get; set; } = "   ";

        [Column("AgingMonth")]
        public int? AgingMonth { get; set; }

        [Column("AgingDays")]
        public int? AgingDays { get; set; }

        [Column("Quarter")]
        public string Quarter { get; set; }

        [Column("PV")]
        public float? PV { get; set; }

        [Column("Loss")]
        public float? Loss { get; set; }

        [Column("TDRDate2")]
        public DateTime? TDRDate2 { get; set; }

        [Column("D_Month")]
        public string D_Month { get; set; }

        [Column("BOTExit")]
        public string BOTExit { get; set; }

        [Column("CountYrMth")]
        public int? CountYrMth { get; set; }

        [Column("YrMth")]
        public string YrMth { get; set; }

        [Column("ConditionPay")]
        public int? ConditionPay { get; set; }

        [Column("ActualPay")]
        public float? ActualPay { get; set; }

        [Column("PayLess")]
        public float? PayLess { get; set; }

        [Column("ResultCode")]
        public string ResultCode { get; set; }

        [Column("MonthClass")]
        public int? MonthClass { get; set; }

        [Column("TDRDate1")]
        public DateTime? TDRDate1 { get; set; }

        [Column("CurrentBal")]
        public float? CurrentBal { get; set; }

        [Column("CurrentAcc")]
        public float? CurrentAcc { get; set; }

        [Column("CurrentUn")]
        public float? CurrentUn { get; set; }

        [Column("EntryDate")]
        public DateTime? EntryDate { get; set; }

        [Column("EditDate")]
        public DateTime? EditDate { get; set; }

        [Column("Haircut")]
        public float? Haircut { get; set; }

        [Column("Loss_Haircut")]
        public float? LossHaircut { get; set; }

        [Column("sResultCode")]
        public string SResultCode { get; set; }

        [Column("sMonthClass")]
        public int? SMonthClass { get; set; }

        [Column("D_Month_Ins")]
        public string DMonthIns { get; set; }
    }
}