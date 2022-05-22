namespace Genbox.SimpleS3.Cli.Exceptions;

public class CliGenericRequiredException : Exception
{
    public CliGenericRequiredException(string message) : base(message) {}
}