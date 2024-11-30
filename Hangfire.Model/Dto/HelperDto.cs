namespace Hangfire.Models.Dto
{
    public class ParameterOfStoredProcedure
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }

    public class AccountAndSuffix
    {
        public string? Account { get; set; }
        public string? Suffix { get; set; }
    }

    public class SftpSettings
    {
        public string Host { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
    }

    public class SFTPConfigModel
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? EncryptPassword { get; set; }
        public string? TDRPathIn { get; set; }
        public string? TDRPathOut { get; set; }
    }

    public enum SFTPFunction
    {
        Download,
        Upload
    }

}
