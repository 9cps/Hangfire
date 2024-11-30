using System.ComponentModel.DataAnnotations;

namespace Hangfire.Models.Entities;

public class BaseTdrMst : BaseTdrMstMethod
{
    [MaxLength(12)]
    public string account_no { get; set; }
    [MaxLength(5)]
    public string suffix_no { get; set; }
    [MaxLength(20)]
    public string ref_no { get; set; }
    [MaxLength(30)]
    public string rm_id { get; set; }
    [MaxLength(150)]
    public string cust_name { get; set; }

    [MaxLength(10)]
    public string chance { get; set; }
    [MaxLength(10)]
    public string ovmstage { get; set; }
    public int? trigger_point { get; set; }
    public DateTime? end_date_relief { get; set; }


    public double? pric_amt_waive { get; set; }
    public double? prin_amt_waive_f { get; set; }
    public double? int_amt_waive { get; set; }
    public double? int_amt_waive_f { get; set; }
    public DateTime? grace_date_prin { get; set; }
    public DateTime? grace_date_int { get; set; }
    public bool asset_flag { get; set; }
    [MaxLength(150)]
    public string asset_n_type { get; set; }
    public double? asset_n_amt { get; set; }
    public double? asset_n_appramt { get; set; }
    public DateTime? asset_date { get; set; }
    [MaxLength(150)]
    public string asset_swap { get; set; }
    public double? asset_prin_amt { get; set; }
    public double? asset_acc_amt { get; set; }
    public double? asset_une_amt { get; set; }
    public double? asset_tot_amt { get; set; }

    public DateTime? edit_date { get; set; }
    [MaxLength(10)]
    public string edit_by { get; set; }
    public DateTime? apr_date { get; set; }
    [MaxLength(10)]
    public string apr_by { get; set; }
    public decimal? orig_pay_schedule { get; set; }
    [MaxLength(15)]
    public string custno { get; set; }
}