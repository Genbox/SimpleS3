namespace Genbox.SimpleS3.Core.Common.Validation;

public class RequireException : Exception
{
    public string CallerArgument { get; }
    public string CallerMember { get; }
    public int LineNumber { get; }

    public RequireException(string callerArgument, string callerMember, int lineNumber, string? message = null) : base(message)
    {
        CallerArgument = callerArgument;
        CallerMember = callerMember;
        LineNumber = lineNumber;
    }
}