using Hangfire.Models.Dto;

namespace Hangfire.Shared.Helper;
public static class DataHelper
{
    public static AccountAndSuffix SplitAccountSuffix(string accountSuffix)
    {
        var result = new AccountAndSuffix();
        if (!string.IsNullOrEmpty(accountSuffix))
        {
            result.Account = accountSuffix.Substring(0, accountSuffix.Length - 3);
            result.Suffix = accountSuffix.Substring(accountSuffix.Length - 3);
        }

        return result;
    }

    public static string SplitZero(this string text, bool isFirst)
    {
        return isFirst ? text.StartsWith('0') ? text.Substring(1) : text : text.Substring(0, text.Length - 1);
    }
}
