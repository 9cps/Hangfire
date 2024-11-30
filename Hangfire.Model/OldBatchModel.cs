using Hangfire.Models.Entities;

namespace Hangfire.Models
{
    public class BatchOldConfig
    {
        public StepConfig Step1 { get; set; }
        public StepConfig Step2 { get; set; }
        public StepConfig Step3 { get; set; }
        public StepConfig Step4 { get; set; }
        public StepConfig Step5 { get; set; }
        public StepConfig Step6 { get; set; }
        public StepConfig Step7 { get; set; }
        public StepConfig Step8 { get; set; }
    }

    public class StepConfig
    {
        public string DeleteFrom1 { get; set; }
        public string DeleteFrom2 { get; set; }
        public string CopyFrom { get; set; }
        public string CopyTo { get; set; }
        public string ExecuteFile { get; set; }
    }

    public class RelationModel : LocZCustIDLast
    {
        public string Flag { get; set; }
        public string SuccessFlag { get; set; }
    }

    //public class ProcessInfo
    //{
    //    public string FileName { get; set; }
    //    public bool UseShellExecute { get; set; }
    //    public bool CreateNoWindow { get; set; }
    //    public bool RedirectStandardOutput { get; set; }
    //    public bool RedirectStandardError { get; set; }
    //}

}
