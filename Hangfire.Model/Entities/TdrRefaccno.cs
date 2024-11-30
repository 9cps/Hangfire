using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities;

[Table("tdr_refaccno")]
[PrimaryKey(nameof(tdr_no), nameof(seq_no))]
public class TdrRefaccno : BaseRefaccnoModel
{
    [Required]
    [MaxLength(30)]
    public string tdr_no { get; set; }
}

public class BaseRefaccnoModel
{
    [Required]
    public int seq_no { get; set; }
    [MaxLength(10)]
    public string old_account { get; set; }
    [MaxLength(5)]
    public string old_suffix_no { get; set; }
    [MaxLength(30)]
    public string old_ref_no { get; set; }
    public decimal? cur_book_bal { get; set; }
    public decimal? orig_pay_schedule { get; set; }
    public int? stage { get; set; }
    public string PD_flag { get; set; }
    public bool flag { get; set; }
}