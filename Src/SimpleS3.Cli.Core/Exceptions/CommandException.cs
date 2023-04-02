using Genbox.SimpleS3.Cli.Core.Enums;

namespace Genbox.SimpleS3.Cli.Core.Exceptions;

public class CommandException : Exception
{
    public CommandException(ErrorType errorType, string message) : base(message)
    {
        ErrorType = errorType;
    }

    public CommandException(ErrorType errorType, string message, string? value) : base(message)
    {
        ErrorType = errorType;
        Value = value;
    }

    public ErrorType ErrorType { get; }

    public string? Value { get; }
}