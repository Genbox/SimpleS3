using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects;

[Command("object")]
[Subcommand(typeof(CopyCommand), typeof(ListCommand), typeof(ListVersionsCommand), typeof(MoveCommand), typeof(RemoveCommand), typeof(SyncCommand))]
internal class Object : CommandBase
{
    protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        app.ShowHelp();
        return Task.CompletedTask;
    }
}