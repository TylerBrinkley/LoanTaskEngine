using Newtonsoft.Json;

namespace LoanTaskEngine.Utilities;

internal static class JsonHelper
{
    public static readonly JsonSerializer DefaultSerializer = new();
}