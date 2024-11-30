using System.ComponentModel.DataAnnotations;

namespace Hangfire.Models
{
    public class RecordPdpa
    {
        [MaxLength(1)]
        public char RecordType { get; set; }
        [MaxLength(14)]
        public string RmNo { get; set; }
        [MaxLength(14)]
        public string AccountSuffix { get; set; }
        //public string SuffixNo { get; set; }
        //public string RefNo { get; set; }
        [MaxLength(2)]
        public string CustomerStatus { get; set; }
    }

    public class PdpaConfig
    {
        public InBoundConfig InBound { get; set; }
        public OutBoundConfig OutBound { get; set; }
    }

    public class InBoundConfig
    {
        public string DefaultPath { get; set; }
        public string UPDFile { get; set; }
        public string DLDFile { get; set; }
        public string Extension { get; set; }
        public string SetDateTimeFile { get; set; }
    }

    public class OutBoundConfig
    {
        public string DefaultPath { get; set; }
        public string File { get; set; }
        public string Extension { get; set; }
        public string SetDateTimeFile { get; set; }
    }
}
