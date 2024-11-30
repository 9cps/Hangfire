using Hangfire.Database;
using Hangfire.Models.Dto;
using Hangfire.Models.Entities;
using Hangfire.Repositories.Interface;
using Hangfire.Shared.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace Hangfire.Repositories
{
    public class BatchOldMigratedRepository : IBatchOldMigratedRepository
    {
        private readonly ILogger<BatchOldMigratedRepository> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public BatchOldMigratedRepository(ILogger<BatchOldMigratedRepository> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public List<LocZCustIDLastRelation> GetListRelations()
        {
            return null;
        }

        public int DataDwhCount()
        {
            try
            {
                return _context.DataDwhDailies.Count();
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error: {ex.Message}");
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                return 0;
            }
        }

        public int DataFtpAlsCount()
        {
            try
            {
                return _context.DataFtpAls.Where(w => w.FlagPost.Equals("N")).Count();
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error: {ex.Message}");
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                return 0;
            }
        }


        public async Task<bool> SFTPFile(string section, string clientFileName, string serverFileName, SFTPFunction func)
        {
            try
            {
                // Get config sftp follow section ==>
                //
                var sftpConfigSection = _configuration.GetSection($"SftpConfig:{section}");
                string? host = sftpConfigSection.GetSection("Host").Value;
                string? user = sftpConfigSection.GetSection("UserID").Value;
                string? password = sftpConfigSection.GetSection("Password").Value;
                string? path = sftpConfigSection.GetSection("Path").Value; // /dwhprod/export/COMS/
                int key = Convert.ToInt32(sftpConfigSection.GetSection("Key").Value);

                if (string.IsNullOrEmpty(host))
                    throw new InvalidOperationException("Host is not configured.");

                // Decrypt password if needed
                if (key > 0)
                    password = await DecryptSftpAsync(password, key);

                // Perform SFTP operation
                using (var sftp = new SftpClient(host, user, password))
                {
                    sftp.Connect();
                    bool result = func switch
                    {
                        SFTPFunction.Download => DownloadFile(sftp, clientFileName, Path.Combine(path, serverFileName)),
                        SFTPFunction.Upload => UploadFile(sftp, clientFileName, Path.Combine(path, serverFileName)),
                        _ => throw new ArgumentOutOfRangeException(nameof(func), func, null)
                    };
                    sftp.Disconnect();
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error: {ex.Message}");
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                return false;
            }
        }

        private bool DownloadFile(SftpClient sftp, string localFilePath, string remoteFilePath)
        {
            try
            {
                using (var fileStream = File.OpenWrite(localFilePath))
                {
                    sftp.DownloadFile(remoteFilePath, fileStream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Download error: {ex.Message}");
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                return false;
            }
        }

        private bool UploadFile(SftpClient sftp, string localFilePath, string remoteFilePath)
        {
            try
            {
                using (var fileStream = File.OpenRead(localFilePath))
                {
                    sftp.UploadFile(fileStream, remoteFilePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload error: {ex.Message}");
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                return false;
            }
        }

        private async Task<string> DecryptSftpAsync(string s, int i)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            var result = string.Empty;
            int length = s.Length;

            while (length > 0)
            {
                var sEncrypt = s.Substring(0, Math.Min(i, length));
                var sDecrypt = await DecodeSftpAsync(sEncrypt, i);
                result += sDecrypt;

                s = s.Substring(Math.Min(i, length));
                length = s.Length;
            }

            return result;
        }

        private async Task<string> DecodeSftpAsync(string s, int i)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            try
            {
                // Using LINQ to query the database
                var result = await _context.Crypts
                    .Where(c => c.Crypt2.Substring(0, Math.Min(i, c.Crypt2.Length)) == s && c.RecordStatus == "A")
                    .Select(c => c.Crypt1)
                    .FirstOrDefaultAsync();

                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                return string.Empty;
            }
        }
    }
}
