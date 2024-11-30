using Hangfire.Models;
using Hangfire.Models.Dto;
using Microsoft.Extensions.Configuration;

namespace Hangfire.Shared.Helper
{
    public static class ConfigurationHelper
    {
        private static IConfiguration? Config;
        private static string? EncryptionKey;

        public static void Initialize(IConfiguration Configuration)
        {
            Config = Configuration;
            EncryptionKey = Config.GetSection("EncryptionKey").Value;
        }

        public static IConfiguration config
        {
            get { return Config ?? throw new InvalidOperationException("Configuration not initialized."); }
        }

        public static string ConnectionString()
        {
            return config.GetConnectionString("DefaultConnection")?.Decrypt(EncryptionKey) ?? "";
        }

        public static SFTPConfigModel ConfigurationMFT(TypeConfig typeConfig = TypeConfig.BATCH_OLD)
        {
            var result = new SFTPConfigModel();

            switch (typeConfig)
            {
                case TypeConfig.BATCH_OLD:
                    result.Host = Config?.GetSection("MFTConnectionStrings:Host").Value;
                    result.Port = Convert.ToInt32(Config?.GetSection("MFTConnectionStrings:Port").Value);
                    result.EncryptPassword = Config?.GetSection("MFTConnectionStrings:EncryptPassword").Value;
                    result.Username = Config?.GetSection("MFTConnectionStrings:Username").Value;
                    result.TDRPathIn = $"{Config?.GetSection("RemotePath:DefaultPath").Value}{Config?.GetSection("RemotePath:TDRInPath").Value}";
                    result.TDRPathOut = $"{Config?.GetSection("RemotePath:DefaultPath").Value}{Config?.GetSection("RemotePath:TDROutPath").Value}";
                    break;
                case TypeConfig.PDPA:
                    result.Host = Config?.GetSection("MFTConnectionStrings:Host").Value;
                    result.Port = Convert.ToInt32(Config?.GetSection("MFTConnectionStrings:Port").Value);
                    result.EncryptPassword = Config?.GetSection("MFTConnectionStrings:EncryptPassword").Value;
                    result.Username = Config?.GetSection("MFTConnectionStrings:Username").Value;
                    result.TDRPathIn = Config?.GetSection("RemotePathPdpa:ImportPath").Value;
                    result.TDRPathOut = Config?.GetSection("RemotePathPdpa:ExportPath").Value;
                    break;
            }
            return result;
        }
    }
}
