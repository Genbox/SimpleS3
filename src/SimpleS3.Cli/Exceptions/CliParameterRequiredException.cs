namespace Genbox.SimpleS3.Cli.Exceptions;

public class CliParameterRequiredException : Exception
{
    public CliParameterRequiredException(string parameterName)
    {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }
}