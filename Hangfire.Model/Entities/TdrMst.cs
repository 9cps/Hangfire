using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities;

[Table("tdr_mst")]
public class TdrMst : BaseTdrMst
{
    [Key]
    [MaxLength(30)]
    public string tdr_no { get; set; }
    public int tdr_seq { get; set; }
    public decimal balance_amt { get; set; }
    public decimal int_rate { get; set; }
    public decimal pv_amt { get; set; }
    public decimal pv_loss { get; set; }
    public int moniter_term { get; set; }
    [MaxLength(1)]
    public string flag_coms { get; set; }
    [MaxLength(1)]
    public string flag_runbatch { get; set; }
    public DateTime? issue_date { get; set; }
    public DateTime? maturity_date { get; set; }
    public bool als_active_status { get; set; }
}