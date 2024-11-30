using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities;

[Table("tdr_scheme")]
[PrimaryKey(nameof(tdr_no), nameof(seq_no))]
public class TdrScheme : BaseTDRSchemeModel
{
    [Required]
    [MaxLength(30)]
    public string tdr_no { get; set; }
}

public class BaseTDRSchemeModel
{
    [Required]
    public int seq_no { get; set; }
    [MaxLength(50)]
    public string scheme { get; set; }
    public DateTime? sche_enddate { get; set; }
}