using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hangfire.Models.Entities
{
    [Table("tblCrypt")]
    public class TblCrypt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Crypt1")]
        [StringLength(1)]
        public string Crypt1 { get; set; }

        [Column("Crypt2")]
        [StringLength(3)]
        public string Crypt2 { get; set; }

        [Column("RecordStatus")]
        [StringLength(1)]
        public string RecordStatus { get; set; }
    }
}
