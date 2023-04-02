using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Profiles;

[Command("profile")]
[Subcommand(typeof(CreateCommand), typeof(ListCommand))]
internal class Profile : CommandBase
{
    protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        app.ShowHelp();
        return Task.CompletedTask;
    }
}