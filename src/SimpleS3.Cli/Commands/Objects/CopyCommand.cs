using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects;

[Command("cp", Description = "Copy one or more objects")]
internal class CopyCommand : ObjectOperationBase
{
    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        await ObjectManager.CopyAsync(Source, Destination).ToListAsync(token);

        Console.WriteLine($"Successfully copied {Source} to {Destination}");
    }
}