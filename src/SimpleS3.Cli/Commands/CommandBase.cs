using Genbox.SimpleS3.Cli.Core;
using Genbox.SimpleS3.Cli.Exceptions;
using Genbox.SimpleS3.Core.Common.Exceptions;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands;

public abstract class CommandBase
{
    protected ServiceManager ServiceManager { get; private set; }
    protected abstract Task ExecuteAsync(CommandLineApplication app, CancellationToken token);

    internal async Task<int> OnExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        S3Cli? s3Cli = null;

        if (app is CommandLineApplication<S3Cli> cliApp)
            s3Cli = cliApp.Model;
        else if (app.Parent is CommandLineApplication<S3Cli> cliApp2)
            s3Cli = cliApp2.Model;
        else if (app.Parent != null && app.Parent.Parent is CommandLineApplication<S3Cli> cliApp3)
            s3Cli = cliApp3.Model;

        if (s3Cli == null)
            throw new S3Exception("Unable to find parent.");

        ServiceManager = ServiceManager.GetInstance(s3Cli.ProfileName, s3Cli.Endpoint, s3Cli.ProxyUrl);

        try
        {
            await ExecuteAsync(app, token).ConfigureAwait(false);
            return 0;
        }
        catch (CliGenericRequiredException ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return 1;
        }
        catch (CliParameterRequiredException ex)
        {
            await Console.Error.WriteLineAsync("Parameter is required: " + ex.ParameterName);
            return 1;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync("An error happened: " + ex.Message);
            return 1;
        }
    }
}