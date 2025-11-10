using System.Diagnostics.CodeAnalysis;

namespace Finviz.TestApp.Domain.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmptyString([NotNullWhen(returnValue: false)] this string? input)
        => string.IsNullOrEmpty(input);
}
