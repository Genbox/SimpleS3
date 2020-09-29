using System;
using Genbox.SimpleS3.Cli.Core.Enums;

namespace Genbox.SimpleS3.Cli.Core.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException(ErrorType errorType, string message) : base(message)
        {
            ErrorType = errorType;
        }

        public ErrorType ErrorType { get; }
    }
}