using System.Runtime.CompilerServices;

namespace LoanTaskEngine.Utilities;

internal static class Preconditions
{
    public static T NotNull<T>(T? value, [CallerArgumentExpression("value")] string? paramName = null) => value ?? throw new ArgumentNullException(paramName);

    public static string NotNullOrEmpty(string? value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        NotNull(value, paramName);
        if (value!.Length == 0)
        {
            throw new ArgumentException("cannot be empty", paramName);
        }
        return value;
    }
}