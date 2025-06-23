using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LasseVK.Extensions;

public static class Assume
{
    [Conditional("DEBUG")]
    public static void That([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression(nameof(condition))] string? expression = null, [CallerMemberName] string? memberName = null, [CallerFilePath] string? sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (!condition)
        {
            Debug.WriteLine($"Assumption failed: {expression} in {memberName} at {sourceFilePath}:{sourceLineNumber}");
        }
    }
}