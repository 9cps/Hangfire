using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Dto
{
    public class ALS10Model
    {
        [Column("Custid")]
        public string Custid { get; set; }

        [Column("Accno")]
        public string Accno { get; set; }

        [Column("Suffix")]
        public string Suffix { get; set; }

        [Column("TDRSeqno")]
        public int? TDRSeqno { get; set; }

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
        public float? Loss_Haircut { get; set; }

        [Column("sResultCode")]
        public string sResultCode { get; set; }

        [Column("sMonthClass")]
        public int? sMonthClass { get; set; }

        [Column("D_Month_Ins")]
        public string D_Month_Ins { get; set; }

        [Column("Flag")]
        public string Flag { get; set; }
    }

    public class ALS10RelationModel
    {

        [Column("CustID")]
        public string CustID { get; set; }

        [Column("AccNo")]
        public string AccNo { get; set; }

        [Column("Suffix")]
        public string Suffix { get; set; }

        [Column("TDRSeqNo")]
        public int? TDRSeqNo { get; set; }

        [Column("ReferNo")]
        public string ReferNo { get; set; }

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
        public float? ConditionPay { get; set; }

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

        [Column("1CustID")]
        public string N1CustID { get; set; }           // เปลี่ยนจาก [1CustID] เป็น N1CustID

        [Column("1AccNo")]
        public string N1AccNo { get; set; }            // เปลี่ยนจาก [1AccNo] เป็น N1AccNo

        [Column("1Suffix")]
        public string N1Suffix { get; set; }           // เปลี่ยนจาก [1Suffix] เป็น N1Suffix

        [Column("1TDRSeqNo")]
        public int? N1TDRSeqNo { get; set; }         // เปลี่ยนจาก [1TDRSeqNo] เป็น N1TDRSeqNo

        [Column("1ReferNo")]
        public string N1ReferNo { get; set; }          // เปลี่ยนจาก [1ReferNo] เป็น N1ReferNo

        [Column("1CurrentBal")]
        public float? N1CurrentBal { get; set; }       // เปลี่ยนจาก [1CurrentBal] เป็น N1CurrentBal

        [Column("1CurrentAcc")]
        public float? N1CurrentAcc { get; set; }       // เปลี่ยนจาก [1CurrentAcc] เป็น N1CurrentAcc

        [Column("1CurrentUn")]
        public float? N1CurrentUn { get; set; }        // เปลี่ยนจาก [1CurrentUn] เป็น N1CurrentUn

        [Column("1TDRDate2")]
        public DateTime? N1TDRDate2 { get; set; }      // เปลี่ยนจาก [1TDRDate2] เป็น N1TDRDate2

        [Column("1D_Month")]
        public string N1D_Month { get; set; }          // เปลี่ยนจาก [1D_Month] เป็น N1D_Month

        [Column("1TDRDate1")]
        public DateTime? N1TDRDate1 { get; set; }      // เปลี่ยนจาก [1TDRDate1] เป็น N1TDRDate1

        [Column("EntryDate")]
        public DateTime? EntryDate { get; set; }

        [Column("1EntryDate")]
        public DateTime? N1EntryDate { get; set; }     // เปลี่ยนจาก [1EntryDate] เป็น N1EntryDate

        [Column("1LoanType")]
        public string N1LoanType { get; set; }         // เปลี่ยนจาก [1LoanType] เป็น N1LoanType

        [Column("1ContType")]
        public string N1ContType { get; set; }         // เปลี่ยนจาก [1ContType] เป็น N1ContType

        [Column("1OldAgeMonth")]
        public int? N1OldAgeMonth { get; set; }      // เปลี่ยนจาก [1OldAgeMonth] เป็น N1OldAgeMonth

        [Column("1OldAgeDays")]
        public int? N1OldAgeDays { get; set; }       // เปลี่ยนจาก [1OldAgeDays] เป็น N1OldAgeDays

        [Column("1AgingMonth")]
        public int? N1AgingMonth { get; set; }       // เปลี่ยนจาก [1AgingMonth] เป็น N1AgingMonth

        [Column("1AgingDays")]
        public int? N1AgingDays { get; set; }        // เปลี่ยนจาก [1AgingDays] เป็น N1AgingDays

        [Column("Haircut")]
        public float? Haircut { get; set; }

        [Column("1Haircut")]
        public float? N1Haircut { get; set; }          // เปลี่ยนจาก [1Haircut] เป็น N1Haircut

        [Column("acc_orig_pay_sch")]
        public decimal? AccOrigPaySch { get; set; }    // เปลี่ยนจาก acc_orig_pay_sch เป็น AccOrigPaySch

        [Column("acc_orig_cur_bookbal")]
        public decimal? AccOrigCurBookBal { get; set; } // เปลี่ยนจาก acc_orig_cur_bookbal เป็น AccOrigCurBookBal
    }
}