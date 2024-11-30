using Hangfire.Models;
using Hangfire.Models.DefaultParameters;
using Hangfire.Models.Dto;
using Hangfire.Models.Entities;
using Hangfire.Repositories.Interface;
using Hangfire.Services.Interface;
using Hangfire.Shared.FileHelper;
using Hangfire.Shared.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Hangfire.Services
{
    public class PdpaService : IPdpaService
    {
        private readonly IPdpaRepository _repo;
        private readonly IBaseRepository _baseRepo;
        private readonly ILogger<PdpaService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string? _asOfDate;
        private readonly string? encryptionKey;
        private readonly PdpaConfig _pdpaConfig;

        public PdpaService(IPdpaRepository repo, IBaseRepository baseRepository, ILogger<PdpaService> logger, IConfiguration configuration, IOptions<PdpaConfig> pdpaConfig)
        {
            _repo = repo;
            _baseRepo = baseRepository;
            _logger = logger;
            _configuration = configuration;
            encryptionKey = configuration.GetSection("EncryptionKey").Value;
            if (configuration.GetSection("Environment").Value == "DEV")
                _asOfDate = configuration.GetSection("PdpaConfig").GetSection("InBound").GetSection("SetDateTimeFile").Value;
            else
                _asOfDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
            _pdpaConfig = pdpaConfig.Value;
        }

        // Initialize Daily
        public async void UpdPdpaInitializeProcessManual(string dateManual)
        {
            try
            {
                // Download UPD file from DWH
                DownloadFileUPDInitialize(dateManual);
                Console.WriteLine("====> Download UPD file from dwh successfully. <====");

                // Read file and insert to temp
                InsertUpdTempAccountPdpa();
                Console.WriteLine("====> Insert temp account upd pdpa successfully. <====");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog("UPD-PDPA Process", $"Error => {ex.Message}", "F");
                throw new Exception(ex.Message);
            }
        }

        public async void DldPdpaInitializeProcessManual(string dateManual)
        {
            try
            {
                // Download DLD file from DWH
                DownloadFileDLD();
                Console.WriteLine("====> Download DLD file from dwh successfully. <====");

                // Read file and insert to temp
                InsertDldTempAccountPdpa();
                Console.WriteLine("====> Insert temp account dld pdpa successfully. <====");

                // Cleansing data ... and insert log fix date
                CleansingAccountPdpa(dateManual);
                Console.WriteLine("====> Cleansing account pdpa successfully. <====");

                // Create log file and sent to dwh
                CreateAndUploadFilePdpa(dateManual);
                Console.WriteLine("====> Create log and upload account pdpa successfully. <====");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog("UPD-PDPA Process", $"Error => {ex.Message}", "F");
                throw new Exception(ex.Message);
            }
        }

        private void DownloadFileUPDInitialize(string dateManual)
        {
            try
            {
                var asOfdate = string.IsNullOrEmpty(dateManual) ? _asOfDate : DateTimeHelper.ConvertDateToTxtFileFormat(DateTimeHelper.ConvertToDate(dateManual));
                var configMFT = ConfigurationHelper.ConfigurationMFT(TypeConfig.PDPA);
                string? sourcePath = $"{_pdpaConfig.InBound.UPDFile}_{asOfdate}{_pdpaConfig.InBound.Extension}";
                string? destinationPath = $"{_pdpaConfig.InBound.DefaultPath}{sourcePath}";

                // UPD File
                FileHelper.Transfer(configMFT, destinationPath, sourcePath, encryptionKey, SFTPFunction.Download);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async void DldPdpaProcessManualInitialize(string dateManual)
        {
            try
            {
                // Download DLD file from DWH
                DownloadFileDLDInitialize(dateManual);
                Console.WriteLine("====> Download DLD file from dwh successfully. <====");

                // Read file and insert to temp
                InsertDldTempAccountPdpaInitialize(dateManual);
                Console.WriteLine("====> Insert temp account dld pdpa successfully. <====");

                // Cleansing data ... and insert log fix date
                CleansingAccountPdpa(dateManual);
                Console.WriteLine("====> Cleansing account pdpa successfully. <====");

                // Create log file and sent to dwh
                CreateAndUploadFilePdpa(dateManual);
                Console.WriteLine("====> Create log and upload account pdpa successfully. <====");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog("UPD-PDPA Process", $"Error => {ex.Message}", "F");
                throw new Exception(ex.Message);
            }
        }

        public async void DldPdpaProcessManual(string dateManual)
        {
            try
            {
                // Download DLD file from DWH
                DownloadFileDLD();
                Console.WriteLine("====> Download DLD file from dwh successfully. <====");

                // Read file and insert to temp
                InsertDldTempAccountPdpa();
                Console.WriteLine("====> Insert temp account dld pdpa successfully. <====");

                // Cleansing data ... and insert log fix date
                CleansingAccountPdpa(dateManual);
                Console.WriteLine("====> Cleansing account pdpa successfully. <====");

                // Create log file and sent to dwh
                CreateAndUploadFilePdpa(dateManual);
                Console.WriteLine("====> Create log and upload account pdpa successfully. <====");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog("UPD-PDPA Process", $"Error => {ex.Message}", "F");
                throw new Exception(ex.Message);
            }
        }

        public async void DldPdpaProcess()
        {
            try
            {
                // Download DLD file from DWH
                DownloadFileDLD();
                Console.WriteLine("====> Download DLD file from dwh successfully. <====");

                // Read file and insert to temp
                InsertDldTempAccountPdpa();
                Console.WriteLine("====> Insert temp account dld pdpa successfully. <====");

                // Cleansing data ... and insert log
                CleansingAccountPdpa();
                Console.WriteLine("====> Cleansing account pdpa successfully. <====");

                // Create log file and sent to dwh
                CreateAndUploadFilePdpa();
                Console.WriteLine("====> Create log and upload account pdpa successfully. <====");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog("UPD-PDPA Process", $"Error => {ex.Message}", "F");
                throw new Exception(ex.Message);
            }
        }

        public async void UpdPdpaProcess()
        {
            try
            {
                // Download UPD file from DWH
                DownloadFileUPD();
                Console.WriteLine("====> Download UPD file from dwh successfully. <====");

                // Read file and insert to temp
                InsertUpdTempAccountPdpa();
                Console.WriteLine("====> Insert temp account upd pdpa successfully. <====");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog("UPD-PDPA Process", $"Error => {ex.Message}", "F");
                throw new Exception(ex.Message);
            }
        }

        private async void InsertUpdTempAccountPdpa()
        {
            try
            {
                // Get config inbound
                var defaultPath = _pdpaConfig.InBound.DefaultPath;
                var fileName = _pdpaConfig.InBound.UPDFile;
                var extension = _pdpaConfig.InBound.Extension;

                string filePath = $"{defaultPath}{fileName}{extension}";

                if (filePath != null)
                {
                    // Read the file and parse it into a list of Record objects0
                    List<RecordPdpa> records = FileHelper.ReadFileToList<RecordPdpa>(filePath, null, _logger);

                    foreach (var obj in records)
                    {
                        Console.WriteLine($"Type: {obj.RecordType}, Field1: {obj.RmNo}, Field2: {obj.AccountSuffix.SplitZero(true)}, Field3: {obj.CustomerStatus}");
                        await _repo.Add(new TempDwhPdpa { AccountSuffix = obj.AccountSuffix.SplitZero(true), ConfirmDel = "WAIT", ConfirmStatus = obj.CustomerStatus });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void InsertDldTempAccountPdpaInitialize(string? dateManual = null)
        {
            try
            {
                // Get config inbound
                var date = string.IsNullOrEmpty(dateManual) ? DateTime.Now.Date : DateTimeHelper.ConvertToDate(dateManual);
                var defaultPath = _pdpaConfig.InBound.DefaultPath;
                var fileName = _pdpaConfig.InBound.DLDFile;
                var extension = _pdpaConfig.InBound.Extension;

                string filePath = $"{defaultPath}{fileName}_{dateManual?.ConvertDateToTxtFileFormat()}{extension}";

                if (filePath != null)
                {
                    // Read the file and parse it into a list of Record objects0
                    List<RecordPdpa> records = FileHelper.ReadFileToList<RecordPdpa>(filePath, null, _logger);

                    foreach (var obj in records)
                    {
                        Console.WriteLine($"Type: {obj.RecordType}, Field1: {obj.RmNo}, Field2: {obj.AccountSuffix.SplitZero(true)}, Field3: {obj.CustomerStatus}");
                        _repo.Add(new TempDwhPdpa { AccountSuffix = obj.AccountSuffix.SplitZero(true), ConfirmDel = "DEL", ConfirmStatus = obj.CustomerStatus, CreateDate = date });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void InsertDldTempAccountPdpa(string? dateManual = null)
        {
            try
            {
                // Get config inbound
                var date = string.IsNullOrEmpty(dateManual) ? DateTime.Now.Date : DateTimeHelper.ConvertToDate(dateManual);
                var defaultPath = _pdpaConfig.InBound.DefaultPath;
                var fileName = _pdpaConfig.InBound.DLDFile;
                var extension = _pdpaConfig.InBound.Extension;

                string filePath = $"{defaultPath}{fileName}{extension}";

                if (filePath != null)
                {
                    // Read the file and parse it into a list of Record objects0
                    List<RecordPdpa> records = FileHelper.ReadFileToList<RecordPdpa>(filePath, null, _logger);

                    foreach (var obj in records)
                    {
                        Console.WriteLine($"Type: {obj.RecordType}, Field1: {obj.RmNo}, Field2: {obj.AccountSuffix.SplitZero(true)}, Field3: {obj.CustomerStatus}");
                        _repo.Add(new TempDwhPdpa { AccountSuffix = obj.AccountSuffix.SplitZero(true), ConfirmDel = "DEL", ConfirmStatus = obj.CustomerStatus, CreateDate = date });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async void CleansingAccountPdpa(string? dateManual = null)
        {
            try
            {
                var date = string.IsNullOrEmpty(dateManual) ? DateTime.Now.Date : DateTimeHelper.ConvertToDate(dateManual);
                var listObj = await _repo.Get(date);
                var tragets = listObj.Where(w => w.ConfirmDel.Equals("DEL")).ToList();

                foreach (var tra in tragets)
                {
                    var accountSuffix = DataHelper.SplitAccountSuffix(tra.AccountSuffix);
                    var param = new List<ParameterOfStoredProcedure>
                    {
                        new ParameterOfStoredProcedure { Key = "input_account_no", Value = accountSuffix.Account },
                        new ParameterOfStoredProcedure { Key = "input_suffix_no", Value = accountSuffix.Suffix }
                    };

                    if (dateManual != null)
                        param.Add(new ParameterOfStoredProcedure { Key = "input_log_date", Value = dateManual });

                    // Call stroed for delete account, insert log and delete in temp
                    _baseRepo.CallStoredProcedure(StoreProcedureName.SP_CLEANSING_PDPA, param);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void CreateAndUploadFilePdpa(string? dateManual = null)
        {
            try
            {
                // Get config outbound
                var asOfdate = string.IsNullOrEmpty(dateManual) ? _asOfDate : dateManual;
                var defaultPath = _pdpaConfig.OutBound.DefaultPath;
                var fileName = _pdpaConfig.OutBound.File;
                var extension = _pdpaConfig.OutBound.Extension;
                string localPath = $"{defaultPath}{fileName}_{asOfdate?.ConvertDateToTxtFileFormat()}{extension}";
                string filePath = $"{fileName}_{asOfdate?.ConvertDateToTxtFileFormat()}{extension}";

                CreateLogFilePdpa(localPath, asOfdate);

                // Upload to MFT
                var configMFT = ConfigurationHelper.ConfigurationMFT(TypeConfig.PDPA);
                FileHelper.Transfer(configMFT, localPath, filePath, encryptionKey, SFTPFunction.Upload);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async void CreateLogFilePdpa(string filePath, string? dateManual = null)
        {
            try
            {
                DateTime asOfDate = !string.IsNullOrEmpty(dateManual) ? DateTimeHelper.ConvertToDate(dateManual) : DateTimeHelper.ConvertToDate(_asOfDate ?? DateTime.Now.Date.ToString("dd/MM/yyyy"));
                var listObj = await _repo.GetLogPdpa(asOfDate);

                StringBuilder sb = new StringBuilder();

                int dataTotal = listObj.Count;
                bool isAppend = false;

                sb.GenerateTxtFileFormDataPdpa(listObj, asOfDate.ToString("yyyyMMdd"));
                sb.GenerateTxtFileSummaryFormDataPdpa(dataTotal, 0, 0, 0, asOfDate.ToString("yyyyMMdd"));
                sb.WriteDataToTextFile(isAppend, filePath);
                sb.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void DownloadFileDLD()
        {
            try
            {
                var configMFT = ConfigurationHelper.ConfigurationMFT(TypeConfig.PDPA);
                string? sourcePath = $"{_pdpaConfig.InBound.DLDFile}{_pdpaConfig.InBound.Extension}";
                string? destinationPath = $"{_pdpaConfig.InBound.DefaultPath}{sourcePath}";

                // DLD File
                FileHelper.Transfer(configMFT, destinationPath, sourcePath, encryptionKey, SFTPFunction.Download);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void DownloadFileDLDInitialize(string? dateManual = null)
        {
            try
            {
                var configMFT = ConfigurationHelper.ConfigurationMFT(TypeConfig.PDPA);
                string? sourcePath = $"{_pdpaConfig.InBound.DLDFile}_{dateManual?.ConvertDateToTxtFileFormat()}{_pdpaConfig.InBound.Extension}";
                string? destinationPath = $"{_pdpaConfig.InBound.DefaultPath}{sourcePath}";

                // DLD File
                FileHelper.Transfer(configMFT, destinationPath, sourcePath, encryptionKey, SFTPFunction.Download);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void DownloadFileUPD()
        {
            try
            {
                var configMFT = ConfigurationHelper.ConfigurationMFT(TypeConfig.PDPA);
                string? sourcePath = $"{_pdpaConfig.InBound.UPDFile}{_pdpaConfig.InBound.Extension}";
                string? destinationPath = $"{_pdpaConfig.InBound.DefaultPath}{sourcePath}";

                // UPD File
                FileHelper.Transfer(configMFT, destinationPath, sourcePath, encryptionKey, SFTPFunction.Download);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}