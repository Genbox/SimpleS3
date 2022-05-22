using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects;

[Command("mv", Description = "Move one or more objects")]
internal class MoveCommand : ObjectOperationBase
{
    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        await ObjectManager.MoveAsync(Source, Destination).ToListAsync(token);

        Console.WriteLine($"Successfully moved {Source} to {Destination}");
    }
}