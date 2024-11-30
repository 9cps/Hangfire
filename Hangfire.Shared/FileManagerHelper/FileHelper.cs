using Hangfire.Models.Dto;
using Hangfire.Models.Entities;
using Hangfire.Shared.Helper;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Hangfire.Shared.FileHelper
{
    public static class StringExtensions
    {
        public static string GetDirectoryFilePath(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            string directoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;

            // Ensure the directory path ends with a directory separator character
            if (!directoryPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                directoryPath += Path.DirectorySeparatorChar;
            }

            return directoryPath;
        }
    }

    public class FileHelper
    {
        private readonly ILogger<FileHelper> _logger;

        public FileHelper(ILogger<FileHelper> logger)
        {
            _logger = logger;
        }

        public static bool Move(string sourcePath, string destinationPath)
        {
            try
            {
                // Check if the source file exists
                if (!File.Exists(sourcePath))
                {
                    Console.WriteLine($"Source file not found: {sourcePath}");
                    return false;
                }

                // Ensure the destination directory exists
                string? destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(destinationDirectory))
                {
                    if (destinationDirectory != null)
                        Directory.CreateDirectory(destinationDirectory);
                }

                // Delete the file at destination path if it exists
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                    Console.WriteLine($"Deleted existing file at {destinationPath}");
                }

                // Move the file
                File.Move(sourcePath, destinationPath);
                Console.WriteLine($"File moved from {sourcePath} to {destinationPath}");
                return true;
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"IO Error: {ioEx.Message}");
            }
            catch (UnauthorizedAccessException authEx)
            {
                Console.WriteLine($"Access Error: {authEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
            return false;
        }

        public static void Delete(string destinationPath)
        {
            // Delete the file at destination path if it exists
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
                Console.WriteLine($"Deleted existing file at {destinationPath}");
            }
        }

        public static bool Verify(string destinationPath)
        {
            // Delete the file at destination path if it exists
            if (File.Exists(destinationPath))
                return true;

            return false;
        }

        public static void Transfer(SFTPConfigModel configModel, string localPath, string fileName, string? encryptionKey, SFTPFunction action)
        {
            using var client = new SftpClient(configModel.Host, configModel.Port, configModel.Username, configModel.EncryptPassword?.Decrypt(encryptionKey));

            try
            {
                // Connect to the SFTP server
                client.Connect();
                Console.WriteLine("Connected to SFTP server.");

                string? remoteDirectory = action.Equals(SFTPFunction.Download) ? configModel.TDRPathOut : configModel.TDRPathIn;
                string? remoteFilePath = remoteDirectory + fileName;

                switch (action)
                {
                    case SFTPFunction.Upload:
                        // Upload operation
                        if (!File.Exists(localPath))
                        {
                            throw new FileNotFoundException($"The local file {localPath} does not exist.");
                        }

                        // Check if the remote directory exists, create it if it doesn't
                        if (!client.Exists(remoteDirectory))
                        {
                            client.CreateDirectory(remoteDirectory);
                            Console.WriteLine($"Created remote directory: {remoteDirectory}");
                        }

                        // Open the local file for reading and upload it
                        using (var fileStream = File.OpenRead(localPath))
                        {
                            client.UploadFile(fileStream, remoteFilePath);
                            Console.WriteLine($"Uploaded file to MFT => {remoteFilePath}");
                        }
                        break;

                    case SFTPFunction.Download:
                        // Ensure the directory exists
                        if (!Directory.Exists(localPath.GetDirectoryFilePath()))
                        {
                            Directory.CreateDirectory(localPath.GetDirectoryFilePath());
                            Console.WriteLine($"Created directory at {localPath.GetDirectoryFilePath()}");
                        }

                        if (File.Exists(localPath))
                        {
                            // Check file and delete fisrt
                            File.Delete(localPath);
                            Console.WriteLine($"Deleted existing file at {localPath}");
                        }

                        // Download operation
                        if (client.Exists(remoteFilePath))
                        {
                            // Download the file from the remote server
                            using (var fileStream = File.Create(localPath))
                            {
                                client.DownloadFile(remoteFilePath, fileStream);
                                Console.WriteLine($"Downloaded file to {localPath}");
                            }
                        }
                        else
                        {
                            throw new FileNotFoundException($"The remote file {remoteFilePath} does not exist.");
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unsupported action: {action}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Consider rethrowing or handling the exception as needed
            }
            finally
            {
                // Ensure the client disconnects even if an error occurs
                if (client.IsConnected)
                {
                    client.Disconnect();
                    Console.WriteLine("Disconnected from SFTP server.");
                }
            }
        }

        public static List<T> ReadFileToList<T>(string filePath, char? separator, ILogger logger) where T : new()
        {
            List<T> records = new List<T>();
            try
            {
                bool isHeadline = true;

                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Get the properties of the type T
                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (string line in lines)
                {
                    var record = new T();

                    if (separator.HasValue)
                    {
                        // Split each line by the specified separator
                        string[] parts = line.Split(separator.Value);

                        // Ensure there are enough parts before creating a Record
                        if (parts.Length == properties.Length)
                        {
                            for (int i = 0; i < properties.Length; i++)
                            {
                                // Assign the value to the property
                                var property = properties[i];
                                var value = parts[i];
                                object convertedValue = Convert.ChangeType(value, property.PropertyType);
                                property.SetValue(record, convertedValue);
                            }

                            records.Add(record);
                        }
                        else
                        {
                            logger.LogInformation("============> PDPA READ HEADER <============");
                            logger.LogInformation($"Line skipped due to field count: {line}");
                        }
                    }
                    else
                    {
                        // Handle skip read headline
                        if (isHeadline)
                        {
                            logger.LogInformation("============> PDPA READ HEADER <============");
                            logger.LogInformation($"Line skipped in head line: {line}");

                            isHeadline = false;
                            continue;
                        }

                        // Calculate total width of fields
                        int startIndex = 0;
                        foreach (var property in properties)
                        {
                            // Get field length from custom attribute or assume default length
                            var lengthAttr = property.GetCustomAttribute<MaxLengthAttribute>();
                            int length = lengthAttr?.Length ?? 0;

                            if (startIndex + length <= line.Length)
                            {
                                var value = line.Substring(startIndex, length).Trim();
                                object convertedValue = Convert.ChangeType(value, property.PropertyType);
                                property.SetValue(record, convertedValue);
                                startIndex += length;
                            }
                        }
                        records.Add(record);
                    }
                }
            }

            catch (IOException ex)
            {
                logger.LogError(ex, "Error while reading file {FilePath}: {Message}", filePath, ex.Message);
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
            }

            return records;
        }
    }

    public static class CreateFileHelper
    {
        #region Genarate file 

        public static void WriteDataToTextFile(this StringBuilder sb, bool isAppend, string localPath)
        {
            using (StreamWriter writer = new StreamWriter(localPath, isAppend, new UTF8Encoding(true)))
            {
                writer.Write(sb.ToString());
            }
        }

        public static void GenerateTxtFileFormDataALS10(this StringBuilder sb, List<ALS10Model> data)
        {
            foreach (var item in data)
            {
                // Add data ..
                // Custid - pad to 15 characters
                sb.Append((item.Custid ?? string.Empty).PadRight(15));

                // Accno - pad to 10 characters
                sb.Append((item.Accno ?? string.Empty).PadRight(10));

                // Suffix - pad to 3 characters
                sb.Append((item.Suffix ?? string.Empty).PadRight(3));

                // TDRSeqno - pad to 6 characters
                sb.Append((item.TDRSeqno?.ToString() ?? string.Empty).PadRight(6));

                // ReferNo - pad to 17 characters
                sb.Append((string.IsNullOrWhiteSpace(item.ReferNo) ? string.Empty : item.ReferNo).PadRight(17));

                // AgingMonth - pad to 6 characters
                sb.Append((item.AgingMonth.ToString() ?? string.Empty).PadRight(6));

                // AgingDays - pad to 4 characters
                sb.Append((item.AgingDays.ToString() ?? string.Empty).PadRight(4));

                // Quarter - pad to 10 characters
                sb.Append((item.Quarter?.ToString() ?? string.Empty).PadRight(10));

                // PV - pad to 21 characters
                sb.Append((item.PV.ToString() ?? string.Empty).PadRight(21));

                // Loss - pad to 21 characters
                sb.Append((item.Loss?.ToString() ?? string.Empty).PadRight(21));

                // TDRDate2 - use FormatDate for consistent date formatting
                sb.Append((FormatDate(item.TDRDate2) ?? string.Empty).PadRight(10) + "0:00:00  ");

                // D_Month - pad to 7 characters
                sb.Append((item.D_Month ?? string.Empty).PadRight(7));

                // BOTExit - pad to 1 character
                sb.Append((item.BOTExit ?? string.Empty).PadRight(1));

                // CountYrMth - pad to 6 characters
                // sb.Append((item.CountYrMth?.ToString() ?? string.Empty).PadRight(6));
                sb.Append(string.Empty.PadRight(6));

                // YrMth - pad to 6 characters
                // sb.Append((item.YrMth ?? string.Empty).PadRight(6));
                sb.Append(string.Empty.PadRight(6));

                // ConditionPay - pad to 22 characters (if empty, pad with ".00")
                sb.Append((item.ConditionPay.GetValueOrDefault().ToString("F2") ?? string.Empty).PadRight(22));

                // ActualPay - pad to 22 characters (if empty, pad with ".00")
                sb.Append((item.ActualPay.GetValueOrDefault().ToString("F2") ?? string.Empty).PadRight(22));

                // PayLess - pad to 22 characters (if empty, pad with ".00")
                sb.Append((item.PayLess.GetValueOrDefault().ToString("F2") ?? string.Empty).PadRight(22));

                // ResultCode - pad to 1 character
                sb.Append((item.ResultCode ?? string.Empty).PadRight(1));

                // MonthClass - pad to 6 characters
                sb.Append((item.MonthClass.GetValueOrDefault().ToString() ?? string.Empty).PadRight(6));

                // TDRDate1 - use FormatDate for consistent date formatting
                sb.Append((FormatDate(item.TDRDate1) ?? string.Empty).PadRight(19));

                // CurrentBal - pad to 22 characters
                sb.Append((item.CurrentBal.GetValueOrDefault().ToString() ?? string.Empty).PadRight(22));

                // CurrentAcc - pad to 22 characters
                sb.Append((item.CurrentAcc.GetValueOrDefault().ToString() ?? string.Empty).PadRight(22));

                // CurrentUn - pad to 22 characters
                sb.Append((item.CurrentUn.GetValueOrDefault().ToString() ?? string.Empty).PadRight(22));

                // EntryDate - use FormatDate for consistent date formatting
                sb.Append((FormatDate(item.EntryDate) ?? string.Empty).PadRight(19));

                // EditDate - use FormatDate for consistent date formatting
                sb.Append((FormatDate(item.EditDate) ?? string.Empty).PadRight(19));

                // Haircut - pad to 23 characters
                sb.Append((item.Haircut.ToString() ?? string.Empty).PadRight(23));

                // Loss_Haircut - pad to 21 characters
                sb.Append((item.Loss_Haircut.ToString() ?? string.Empty).PadRight(21));

                // sResultCode - pad to 1 character
                sb.Append((item.sResultCode ?? string.Empty).PadRight(1));

                // sMonthClass - pad to 6 characters
                sb.Append((item.sMonthClass.GetValueOrDefault().ToString() ?? string.Empty).PadRight(6));

                // D_Month_Ins - pad to 7 characters
                sb.Append((item.D_Month_Ins ?? string.Empty).PadRight(7));

                // FLAG - pad to 1 character
                sb.Append((item.Flag ?? string.Empty).PadRight(1));

                // Add a new line for each record
                sb.AppendLine();
            }
        }

        public static void GenerateTxtFileFormDataALS10Relation(this StringBuilder sb, List<ALS10RelationModel> data)
        {
            foreach (var item in data)
            {
                // Start appending each field with padding
                sb.Append(item.CustID.PadRight(15));                // Custid
                sb.Append(item.AccNo.PadRight(10));                  // Accno
                sb.Append(item.Suffix.PadRight(3));                  // Suffix
                sb.Append((item.TDRSeqNo.GetValueOrDefault().ToString() ?? "0").PadRight(6));                // TDRSeqno
                sb.Append(item.ReferNo.PadRight(17));                // ReferNo
                sb.Append((item.AgingMonth.GetValueOrDefault().ToString() ?? "0").PadRight(6));              // AgingMonth
                sb.Append((item.AgingDays.GetValueOrDefault().ToString() ?? "0").PadRight(4));               // AgingDays
                sb.Append(item.Quarter.PadRight(10));                // Quarter
                sb.Append((item.PV.GetValueOrDefault().ToString() ?? "0").PadRight(21));                     // PV
                sb.Append((item.Loss.GetValueOrDefault().ToString() ?? "0").PadRight(21));                   // Loss

                // Date Formatting for 1TDRDate1 and 1TDRDate2
                sb.Append((string.IsNullOrEmpty(FormatDate(item.TDRDate1)) ? FormatDate(item.TDRDate2) : FormatDate(item.TDRDate1)).PadRight(19));   // 1TDRDate1
                sb.Append((item.D_Month ?? string.Empty).PadRight(7));                 // D_Month
                sb.Append((item.BOTExit ?? string.Empty).PadRight(1));                 // BOTExit
                sb.Append((item.CountYrMth.GetValueOrDefault().ToString().Equals("0") ? string.Empty : item.CountYrMth.GetValueOrDefault().ToString()).PadRight(6));              // CountYrMth
                sb.Append((item.YrMth ?? string.Empty).PadRight(6));                   // YrMth
                sb.Append((item.ConditionPay.GetValueOrDefault().ToString().Equals("0") ? string.Empty : item.ConditionPay.GetValueOrDefault().ToString()).PadRight(16));           // ConditionPay
                sb.Append((item.ActualPay.GetValueOrDefault().ToString().Equals("0") ? string.Empty : item.ActualPay.GetValueOrDefault().ToString()).PadRight(25));              // ActualPay
                sb.Append((item.PayLess.GetValueOrDefault().ToString().Equals("0") ? string.Empty : item.PayLess.GetValueOrDefault().ToString()).PadRight(25));                // PayLess
                sb.Append((item.ResultCode ?? string.Empty).PadRight(1));              // ResultCode
                sb.Append((item.MonthClass.GetValueOrDefault().ToString() ?? string.Empty).PadRight(6));              // MonthClass

                // Date Formatting for TDRDate1
                sb.Append((FormatDate(item.TDRDate1) ?? string.Empty).PadRight(19));   // TDRDate1

                // More fields
                sb.Append((item.CurrentBal.GetValueOrDefault().ToString() ?? "0").PadRight(23));             // CurrentBal
                sb.Append((item.CurrentAcc.GetValueOrDefault().ToString() ?? "0").PadRight(21));             // CurrentAcc
                sb.Append((item.CurrentUn.GetValueOrDefault().ToString() ?? "0").PadRight(22));              // CurrentUn
                sb.Append((item.N1CustID ?? string.Empty).PadRight(15));                // 1Custid
                sb.Append((item.N1AccNo ?? string.Empty).PadRight(10));                 // 1Accno
                sb.Append((item.N1Suffix ?? string.Empty).PadRight(3));                 // 1Suffix
                sb.Append((item.N1TDRSeqNo.GetValueOrDefault().ToString() ?? string.Empty).PadRight(6));               // 1TDRSeqno
                sb.Append((item.N1ReferNo ?? string.Empty).PadRight(17));               // 1ReferNo
                sb.Append((item.N1CurrentBal.GetValueOrDefault().ToString() ?? string.Empty).PadRight(23));            // 1CurrentBal
                sb.Append((item.N1CurrentAcc.GetValueOrDefault().ToString() ?? string.Empty).PadRight(21));            // 1CurrentAcc
                sb.Append((item.N1CurrentUn.GetValueOrDefault().ToString() ?? string.Empty).PadRight(22));             // 1CurrentUn
                sb.Append((FormatDate(item.N1TDRDate2) ?? string.Empty).PadRight(19));   // 1TDRDate2
                sb.Append((item.N1D_Month ?? string.Empty).PadRight(7));                // 1D_Month

                // Date Formatting for 1TDRDate1
                sb.Append((FormatDate(item.N1TDRDate1) ?? string.Empty).PadRight(19));   // 1TDRDate1

                // Dates formatting for EntryDate and 1EntryDate
                sb.Append((FormatDate(item.EntryDate) ?? string.Empty).PadRight(19));  // EntryDate
                sb.Append((FormatDate(item.N1EntryDate) ?? string.Empty).PadRight(19)); // 1EntryDate

                sb.Append((item.N1LoanType ?? string.Empty).PadRight(2));               // 1LoanType
                sb.Append((item.N1ContType ?? string.Empty).PadRight(4));               // 1ContType
                sb.Append((item.N1OldAgeMonth.GetValueOrDefault().ToString() ?? string.Empty).PadRight(6));            // 1OldAgeMonth
                sb.Append((item.N1OldAgeDays.GetValueOrDefault().ToString() ?? string.Empty).PadRight(6));             // 1OldAgeDays
                sb.Append((item.N1AgingMonth.GetValueOrDefault().ToString() ?? string.Empty).PadRight(6));             // 1AgingMonth
                sb.Append((item.N1AgingDays.GetValueOrDefault().ToString() ?? string.Empty).PadRight(6));              // 1AgingDays
                sb.Append((item.Haircut.GetValueOrDefault().ToString().Equals("0") ? string.Empty : item.Haircut.GetValueOrDefault().ToString()).PadRight(22));                // Haircut
                sb.Append((item.AccOrigCurBookBal.GetValueOrDefault().ToString().Equals("0") ? string.Empty : item.AccOrigCurBookBal.GetValueOrDefault().ToString()).PadRight(22));      // acc_orig_cur_bookbal
                sb.Append((item.AccOrigPaySch.GetValueOrDefault().ToString().Equals("0") ? string.Empty : item.AccOrigPaySch.GetValueOrDefault().ToString()).PadRight(22));          // acc_orig_pay_sch

                // Append the generated data for this item as a line in the file
                sb.AppendLine();
            }
        }

        // Helper method to format date (returning empty string if null or empty)
        private static string FormatDate(DateTime? date)
        {
            if (date == null)
                return string.Empty; // Return 19 spaces for empty dates
            return date.Value.ToString("d/M/yyyy");
        }

        public static void GenerateTxtFileFormDataPdpa(this StringBuilder sb, List<TdrPdpaDelLog> data, string? asOfDate)
        {
            foreach (var item in data)
            {
                // split string
                var accountSuffix = DataHelper.SplitAccountSuffix(item.AccountSuffix);

                sb.Append($"D");
                sb.Append($"|{asOfDate}");
                sb.Append($"|0000000000000");
                sb.Append($"|{item.CustName}");
                sb.Append($"|");
                sb.Append($"|000");
                sb.Append($"|000");
                sb.Append($"|000");
                sb.Append($"|000");
                sb.Append($"|{accountSuffix.Account}");
                sb.Append($"|{accountSuffix.Suffix}");
                sb.Append($"|00000000000000000");
                sb.Append($"|{accountSuffix.Account}");
                sb.Append($"|A0507");
                sb.Append($"|TDR");
                sb.Append($"|{item.DelDate.ToString("yyyyMMdd")}");
                sb.Append($"|UD");
                sb.Append($"|");
                sb.Append($"|A");
                sb.AppendLine();
            }
        }

        public static void GenerateTxtFileSummaryFormDataPdpa(this StringBuilder sb, int totalRec, decimal sumAmount1, decimal sumAmount2, decimal sumAmount3, string? asOfDate)
        {
            sb.AppendLine($"T|{asOfDate}|{totalRec}|{sumAmount1}|{sumAmount2}|{sumAmount3}|{asOfDate}|TDR_UPD_PDPA_DELETE_LOG_{asOfDate}.TXT");
        }

        #endregion
    }
}
