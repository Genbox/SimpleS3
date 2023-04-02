using Genbox.SimpleS3.Cli.Core.Managers;
using Genbox.SimpleS3.Cli.Exceptions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands;

public abstract class OnlineCommandBase : CommandBase
{
    protected BucketManager BucketManager => ServiceManager.BucketManager;
    protected ObjectManager ObjectManager => ServiceManager.ObjectManager;

    protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        if (app.Parent!.Parent is not CommandLineApplication<S3Cli> parent)
            throw new InvalidOperationException("Unable to find parent");

        S3Cli cli = parent.Model;

        if (cli.ProfileName == null)
            throw new CliParameterRequiredException("-p <profile>");

        IProfile? profile = ServiceManager.ProfileManager.GetProfile(cli.ProfileName);

        if (profile == null)
            throw new CliGenericRequiredException($"You have not yet setup the profile '{cli.ProfileName}'");

        return Task.CompletedTask;
    }
}