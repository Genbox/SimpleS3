using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets;

[Command("bucket")]
[Subcommand(typeof(CreateCommand), typeof(RemoveCommand), typeof(EmptyCommand), typeof(ListCommand))]
internal class Bucket : OnlineCommandBase
{
    protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        app.ShowHelp();
        return Task.CompletedTask;
    }
}