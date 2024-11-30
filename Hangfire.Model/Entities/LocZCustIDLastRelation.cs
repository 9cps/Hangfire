using System.ComponentModel.DataAnnotations.Schema;

// REMARK:  Batch result table
namespace Hangfire.Models.Entities
{
    [Table("Loc_zCustIDLast_Relation")]
    public class LocZCustIDLastRelation
    {
        [Column("CustID")]
        public string CustID { get; set; }

        [Column("AccNo")]
        public string AccNo { get; set; }

        [Column("Suffix")]
        public string Suffix { get; set; }

        [Column("TDRSeqNo")]
        public short? TDRSeqNo { get; set; }

        [Column("ReferNo")]
        public string ReferNo { get; set; } = "   ";

        [Column("AgingMonth")]
        public short? AgingMonth { get; set; }

        [Column("AgingDays")]
        public short? AgingDays { get; set; }

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
        public short? CountYrMth { get; set; }

        [Column("YrMth")]
        public string YrMth { get; set; }

        [Column("ConditionPay")]
        public float? ConditionPay { get; set; }

        [Column("ActualPay")]
        public float? ActualPay { get; set; }

        [Column("PayLess")]
        public float? PayLess { get; set; }

        [Column("ResultCode")]
        public string ResultCode { get; set; }

        [Column("MonthClass")]
        public short? MonthClass { get; set; }

        [Column("TDRDate1")]
        public DateTime? TDRDate1 { get; set; }

        [Column("CurrentBal")]
        public float? CurrentBal { get; set; }

        [Column("CurrentAcc")]
        public float? CurrentAcc { get; set; }

        [Column("CurrentUn")]
        public float? CurrentUn { get; set; }

        [Column("1CustID")]
        public string CustID1 { get; set; }

        [Column("1AccNo")]
        public string AccNo1 { get; set; }

        [Column("1Suffix")]
        public string Suffix1 { get; set; }

        [Column("1TDRSeqNo")]
        public short? TDRSeqNo1 { get; set; }

        [Column("1ReferNo")]
        public string ReferNo1 { get; set; }

        [Column("1CurrentBal")]
        public float? CurrentBal1 { get; set; }

        [Column("1CurrentAcc")]
        public float? CurrentAcc1 { get; set; }

        [Column("1CurrentUn")]
        public float? CurrentUn1 { get; set; }

        [Column("1TDRDate2")]
        public DateTime? TDRDate21 { get; set; }

        [Column("1D_Month")]
        public string D_Month1 { get; set; }

        [Column("1TDRDate1")]
        public DateTime? TDRDate11 { get; set; }

        [Column("EntryDate")]
        public DateTime? EntryDate { get; set; }

        [Column("1EntryDate")]
        public DateTime? EntryDate1 { get; set; }

        [Column("1LoanType")]
        public string LoanType1 { get; set; }

        [Column("1ContType")]
        public string ContType1 { get; set; }

        [Column("1OldAgeMonth")]
        public short? OldAgeMonth1 { get; set; }

        [Column("1OldAgeDays")]
        public short? OldAgeDays1 { get; set; }

        [Column("1AgingMonth")]
        public short? AgingMonth1 { get; set; }

        [Column("1AgingDays")]
        public short? AgingDays1 { get; set; }

        [Column("Haircut")]
        public float? Haircut { get; set; }

        [Column("1Haircut")]
        public float? Haircut1 { get; set; }

        [Column("acc_orig_pay_sch")]
        public decimal? AccOrigPaySch { get; set; }

        [Column("acc_orig_cur_bookbal")]
        public decimal? AccOrigCurBookBal { get; set; }
    }
}