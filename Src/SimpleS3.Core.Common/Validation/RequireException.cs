using System.Diagnostics.CodeAnalysis;
namespace Genbox.SimpleS3.Core.Common.Validation;

/// <summary>Provides the <c>RequireException</c> API.</summary>
[SuppressMessage("Design", "CA1032", Justification = "This exception requires the original argument, member, and line number to describe a failed requirement.")]
public class RequireException(string callerArgument, string callerMember, int lineNumber, string? message = null) : Exception(message)
{
    /// <summary>Provides the <c>CallerArgument</c> API.</summary>
    public string CallerArgument { get; } = callerArgument;
    /// <summary>Provides the <c>CallerMember</c> API.</summary>
    public string CallerMember { get; } = callerMember;
    /// <summary>Provides the <c>LineNumber</c> API.</summary>
    public int LineNumber { get; } = lineNumber;
}