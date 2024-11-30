using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

// REMARK:  Batch result table
namespace Hangfire.Models.Entities
{
    [Table("TblAccount")]
    [PrimaryKey(nameof(CustID), nameof(TDRSeqNo), nameof(AccNo), nameof(Suffix), nameof(ReferNo), nameof(FlagOld))]
    public class TblAccount
    {
        public string CustID { get; set; }
        public int TDRSeqNo { get; set; }
        public string AccNo { get; set; }
        public string Suffix { get; set; }
        public string ReferNo { get; set; }
        public string AccCode { get; set; }
        public bool TDRFlag { get; set; }
        public string BusType { get; set; }
        public decimal BeginBal { get; set; }
        public decimal BeginAcc { get; set; }
        public decimal BeginUn { get; set; }
        public decimal CurrentBal { get; set; }
        public decimal CurrentAcc { get; set; }
        public decimal CurrentUn { get; set; }
        public decimal BadDebtBal { get; set; }
        public decimal BadDebtAcc { get; set; }
        public string MiscTDR { get; set; }
        public string Misc98 { get; set; }
        public string FlagMisc { get; set; }
        public string FlagNPL { get; set; }
        public int AgingMonth { get; set; }
        public int AgingDays { get; set; }
        public string Creater { get; set; }
        public DateTime EntryDate { get; set; }
        public string LastEditor { get; set; }
        public DateTime EditDate { get; set; }
        public string ROCode { get; set; }
        public string LastClass { get; set; }
        public string LastClass2 { get; set; }
        public string LoanType { get; set; }
        public string ContType { get; set; }
        public int OldAgeMonth { get; set; }
        public int OldAgeDays { get; set; }
        public int Prevclass { get; set; }
        public int PrevAging { get; set; }
        public int maxPrevAging { get; set; }
        public int FlagClose { get; set; }
        public string FlagOld { get; set; }
        public decimal acc_orig_pay_sch { get; set; }
        public decimal acc_orig_cur_bookbal { get; set; }
    }
}