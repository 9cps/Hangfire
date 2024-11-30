using Hangfire.Models.Dto;

namespace Hangfire.Repositories.Interface
{
    public interface IBatchOldMigratedRepository
    {
        public int DataDwhCount();
        public int DataFtpAlsCount();
        Task<bool> SFTPFile(string section, string clientFileName, string serverFileName, SFTPFunction func);
    }
}
