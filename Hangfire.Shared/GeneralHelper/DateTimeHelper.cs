using System.Globalization;

namespace Hangfire.Shared.Helper;
public static class DateTimeHelper
{
    public static string GetAsOfDate()
    {
        return DateTime.Now.Date.AddDays(-1).ConvertDateToTxtFileFormat();
    }

    public static string ConvertDateToTxtFileFormat(this string date)
    {
        return ConvertToDate(date).ToString("yyyyMMdd");
    }

    public static string ConvertDateToTxtFileFormat(this DateTime date)
    {
        return date.ToString("yyyyMMdd");
    }

    public static DateTime ConvertToDate(string date)
    {
        // Define the input format
        string inputFormat = "dd/MM/yyyy";

        // Try to parse the input date string
        if (DateTime.TryParseExact(date, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            // Return the parsed DateTime object
            return parsedDate;
        }
        else
        {
            // Return a default value or throw an exception if the format is invalid
            throw new ArgumentException("Invalid date format. Please use dd/MM/yyyy.");
        }
    }
}
