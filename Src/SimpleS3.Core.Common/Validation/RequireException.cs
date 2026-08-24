namespace Genbox.SimpleS3.Core.Common.Validation;

/// <summary>Provides the <c>RequireException</c> API.</summary>
public class RequireException(string callerArgument, string callerMember, int lineNumber, string? message = null) : Exception(message)
{
    /// <summary>Provides the <c>CallerArgument</c> API.</summary>
    public string CallerArgument { get; } = callerArgument;
    /// <summary>Provides the <c>CallerMember</c> API.</summary>
    public string CallerMember { get; } = callerMember;
    /// <summary>Provides the <c>LineNumber</c> API.</summary>
    public int LineNumber { get; } = lineNumber;
}