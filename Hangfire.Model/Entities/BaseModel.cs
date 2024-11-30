using System.ComponentModel.DataAnnotations;

namespace Hangfire.Models.Entities;

public class BaseModel : BaseStatus
{

    public DateTime? create_date { get; set; }
    [MaxLength(10)]
    public string create_by { get; set; }
}

public class BaseStatus
{
    [MaxLength(1)]
    public string recsts { get; set; }
}
