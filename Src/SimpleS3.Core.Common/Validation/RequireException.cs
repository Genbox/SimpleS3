namespace Genbox.SimpleS3.Core.Common.Validation;

public class RequireException(string callerArgument, string callerMember, int lineNumber, string? message = null) : Exception(message)
{
    public string CallerArgument { get; } = callerArgument;
    public string CallerMember { get; } = callerMember;
    public int LineNumber { get; } = lineNumber;
}