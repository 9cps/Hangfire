using Hangfire.Models;
using Hangfire.Models.DefaultParameters;
using Hangfire.Models.Dto;
using Hangfire.Repositories.Interface;
using Hangfire.Services.Interface;
using Hangfire.Shared.FileHelper;
using Hangfire.Shared.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;

namespace Hangfire.Services
{
    public class BatchOldMigratedService : IBatchOldMigratedService
    {
        private readonly IBatchOldMigratedRepository _batchRepo;
        private readonly IBaseRepository _baseRepo;
        private readonly ILogger<BatchOldMigratedService> _logger;
        private readonly IConfiguration _configuration;
        private readonly BatchOldConfig _batchOldConfig;
        public readonly string? _encryptionKey;
        public readonly SFTPConfigModel _mftConfig;

        public BatchOldMigratedService(IBatchOldMigratedRepository batchRepo, IBaseRepository baseRepo, ILogger<BatchOldMigratedService> logger, IConfiguration configuration, IOptions<BatchOldConfig> batchOldConfig)
        {
            _batchRepo = batchRepo;
            _baseRepo = baseRepo;
            _logger = logger;
            _configuration = configuration;
            _encryptionKey = configuration.GetSection("EncryptionKey").Value;
            _batchOldConfig = batchOldConfig.Value;
            _mftConfig = ConfigurationHelper.ConfigurationMFT();
        }

        public void DoWorkOldBatchProcess()
        {
            // ไม่ใส่ await เนื่องจากต้องรอให้ทำเสร็จทีละ Step
            //
            // _ = CloseBranch();
            // _ = ReciveOldAccount();
            //_ = ImportOldAccount();
            //_ = LoadAndImportFileALS();
            //_ = ReciveMisBotClass();
            //_ = GenerateDataDailies();
            _ = GenerateRelationALS10();
        }

        // Running Seq Step ==> 1
        private async Task<bool> CloseBranch()
        {
            try
            {
                SaveProcessLog("1", OldBatchStepName.Step1, "Start", "0");
                string? sourcePath = _batchOldConfig.Step1.CopyFrom;
                string? destinationPath = _batchOldConfig.Step1.CopyTo;

                if (sourcePath != null && destinationPath != null)
                {
                    if (FileHelper.Move(sourcePath, destinationPath))
                    {
                        // Insert Step
                        //
                        Console.WriteLine("====> Moved file succesfully. <====");
                        await _baseRepo.BatchExceptionLog(OldBatchStepName.Step1, "Get File close branch Complete.", "C");

                        // Exec IMPORT_CLOSEBRANCH_TXT
                        _baseRepo.CallStoredProcedure(StoreProcedureName.IMPORT_CLOSEBRANCH_TXT, null);
                        string? deletePath = _batchOldConfig.Step1.DeleteFrom1;
                        if (deletePath != null)
                            FileHelper.Delete(deletePath);

                        SaveProcessLog("2", OldBatchStepName.Step1, "End", "0");
                        return true;
                    }
                    else
                    {
                        // Insert Step
                        //
                        Console.WriteLine("====> Moved file failed. <====");
                        await _baseRepo.BatchExceptionLog(OldBatchStepName.Step1, "Not Close branch.", "C");
                        SaveProcessLog("2", $"{OldBatchStepName.Step1}_ELSE", "End", "0");
                        return true;
                    }
                }
                else
                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step1, $"sourcePath:{sourcePath}, destinationPath:{destinationPath}", "F");

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog(OldBatchStepName.Step1, ex.Message, "F");
                return false;
            }
        }

        // Running Seq Step ==> 2
        private async Task<bool> ReciveOldAccount()
        {
            try
            {
                SaveProcessLog("1", OldBatchStepName.Step2, "Start", "0");
                string? sourcePath = _batchOldConfig.Step2.CopyFrom;
                string? destinationPath = _batchOldConfig.Step1.CopyTo;

                if (!string.IsNullOrEmpty(destinationPath) && !string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(_encryptionKey))
                {
                    FileHelper.Transfer(_mftConfig, destinationPath, sourcePath, _encryptionKey, SFTPFunction.Download);
                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step2, "Reciept file TDR_ACCT_LIST complete.", "C");
                    SaveProcessLog("2", "Step 2.ReciveOldAccount New", "End", "0");
                }
                else
                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step2, "Reciept file TDR_ACCT_LIST fail.", "F");


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog(OldBatchStepName.Step2, ex.Message, "F");
                return false;
            }
        }

        // Running Seq Step ==> 3
        private async Task<bool> ImportOldAccount()
        {
            try
            {
                SaveProcessLog("1", OldBatchStepName.Step3, "Start", "0");

                _baseRepo.CallStoredProcedure(StoreProcedureName.IMPORT_DWH_DAILY, null);

                if (_batchRepo.DataDwhCount() == 0)
                {
                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step3, "Import data TDR_ACCT_LIST fail.", "F");
                    return false;
                }
                else
                {
                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step3, "Import data TDR_ACCT_LIST complete.", "C");
                    SaveProcessLog("2", OldBatchStepName.Step3, "End", "0");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog(OldBatchStepName.Step3, ex.Message, "F");
                return false;
            }
        }

        // Running Seq Step ==> 4
        private async Task<bool> LoadAndImportFileALS()
        {
            try
            {
                string? tragetPath = _batchOldConfig.Step4.CopyFrom;
                string? destinationPath = _batchOldConfig.Step4.CopyTo;
                SaveProcessLog("1", OldBatchStepName.Step3, "Start", "0");

                if (FileHelper.Verify(tragetPath))
                {
                    FileHelper.Move(tragetPath, destinationPath);
                    // await _baseRepo.CallStoredProcedure(StoreProcedureName.IMPORT_ALS_TXT, null);
                    FileHelper.Delete(tragetPath);

                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step3, "Get file ALS complete.", "C");
                    SaveProcessLog("2", OldBatchStepName.Step3, "End", "0");
                }
                else
                {
                    SaveProcessLog("2", OldBatchStepName.Step3, "End", "FTDRLN1.TXT text file not found.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog(OldBatchStepName.Step3, ex.Message, "F");
                return false;
            }
        }

        // Running Seq Step ==> 5
        private async Task<bool> ReciveMisBotClass()
        {
            try
            {
                SaveProcessLog("1", OldBatchStepName.Step4, "Start", "0");
                int iDay = DateTime.Now.Day;
                if (iDay == 6)
                {
                    string? tragetDeletePath = _batchOldConfig.Step5.DeleteFrom1;
                    string? destinationPath = _batchOldConfig.Step5.CopyTo;
                    string? tragetPath = _batchOldConfig.Step5.CopyFrom;

                    FileHelper.Delete(tragetDeletePath);

                    // Download TDR_CLASS.TXT
                    FileHelper.Transfer(_mftConfig, destinationPath, tragetPath, _encryptionKey, SFTPFunction.Download);
                    _baseRepo.CallStoredProcedure(StoreProcedureName.IMPORT_DWH_TXT, null);

                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step5, "Receipt file class complete.", "C");
                    SaveProcessLog("2", OldBatchStepName.Step5, "End", "0");
                }
                else
                {
                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step5, "Receipt file class 1 - 5 days.", "C");
                    SaveProcessLog("2", $"{OldBatchStepName.Step5}_DAY_{iDay}", "End", "0");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog(OldBatchStepName.Step5, ex.Message, "F");
                return false;
            }
        }

        // Running Seq Step ==> 6
        private async Task<bool> GenerateDataDailies()
        {
            try
            {
                SaveProcessLog("1", OldBatchStepName.Step6, "Start", "0");
                _baseRepo.CallStoredProcedure(StoreProcedureName.EXECUTE_PROCEDURE, null);

                if (_batchRepo.DataFtpAlsCount() > 150)
                {
                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step6, "Cannot generate data daily.", "F");
                    SaveProcessLog("2", $"{OldBatchStepName.Step6}_FAIL", "End", "0");
                    return false;
                }
                else
                {
                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step6, "Generate data daily complete.", "C");
                    SaveProcessLog("2", OldBatchStepName.Step6, "End", "0");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog(OldBatchStepName.Step6, ex.Message, "F");
                return false;
            }
        }

        // Running Seq Step ==> 7
        private async Task<bool> GenerateRelationALS10()
        {
            try
            {
                SaveProcessLog("1", OldBatchStepName.Step7, "Start", "0");

                string? tragetDeletePath1 = _batchOldConfig.Step7.DeleteFrom1;
                string? tragetDeletePath2 = _batchOldConfig.Step7.DeleteFrom2;
                FileHelper.Delete(tragetDeletePath1);
                FileHelper.Delete(tragetDeletePath2);

                // Execute sp
                // _baseRepo.CallStoredProcedure(StoreProcedureName.GENERATE_ALS_JOB, null);

                var data = _baseRepo.CallStoredProcedure<ALS10Model>(StoreProcedureName.GEN_TDR_ALS10, null);
                var dataRelations = _baseRepo.CallStoredProcedure<ALS10RelationModel>(StoreProcedureName.GEN_TDR_ALS10_RELATION, null);
                CreateFileALS10(data);
                CreateFileRelationALS10(dataRelations);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog(OldBatchStepName.Step7, ex.Message, "F");
                return false;
            }
        }


        // Running Seq Step ==> 8
        private async Task<bool> SendRelation()
        {
            try
            {
                SaveProcessLog("1", OldBatchStepName.Step8, "Start", "0");
                var processInfo = new ProcessStartInfo
                {
                    FileName = _batchOldConfig.Step8.ExecuteFile,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = new Process { StartInfo = processInfo })
                {
                    process.Start();
                    process.WaitForExit();

                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();

                    await _baseRepo.BatchExceptionLog(OldBatchStepName.Step8, "Generate file Relation complete.", "C");
                    SaveProcessLog("2", OldBatchStepName.Step8, "End", "0");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
                await _baseRepo.BatchExceptionLog(OldBatchStepName.Step8, ex.Message, "F");
                return false;
            }
        }

        private void SaveProcessLog(string seqTask, string prosTask, string prosMessage, string prosSts)
        {
            try
            {
                var listParams = new List<ParameterOfStoredProcedure>
                {
                   new ParameterOfStoredProcedure { Key = "seq_task", Value = seqTask},
                   new ParameterOfStoredProcedure { Key = "pros_task", Value = prosTask},
                   new ParameterOfStoredProcedure { Key = "pros_message", Value = prosMessage},
                   new ParameterOfStoredProcedure { Key = "pros_sts", Value = prosSts},
                };

                _baseRepo.CallStoredProcedure(StoreProcedureName.SP_PROCESS_LOG, listParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Method}: {ErrorMessage}", MethodHelper.CurrentMethodName(), ex.Message);
            }
        }

        private void CreateFileALS10(List<ALS10Model> data)
        {
            StringBuilder sb = new StringBuilder();
            bool isAppend = false;
            sb.GenerateTxtFileFormDataALS10(data);
            sb.WriteDataToTextFile(isAppend, _batchOldConfig.Step7.CopyTo);
            sb.Clear();
        }

        private void CreateFileRelationALS10(List<ALS10RelationModel> data)
        {
            StringBuilder sb = new StringBuilder();
            bool isAppend = false;
            sb.GenerateTxtFileFormDataALS10Relation(data);
            sb.WriteDataToTextFile(isAppend, _batchOldConfig.Step7.CopyFrom);
            sb.Clear();
        }
    }
}