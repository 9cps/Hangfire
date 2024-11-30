using System.Reflection;

namespace Hangfire.Shared.Helper;
public class MethodHelper
{
    public static string CurrentMethodName()
    {
        var method = MethodBase.GetCurrentMethod();
        return method?.Name ?? "UnknownMethod";
    }
}
