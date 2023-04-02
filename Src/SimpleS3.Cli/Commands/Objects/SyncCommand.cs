using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects;

[Command("sync", Description = "Sync changes between a local folder and a remote bucket")]
internal class SyncCommand : ObjectOperationBase
{
    [Option("-c|--concurrent", Description = "Number of concurrent uploads/downloads")]
    public int Concurrent { get; set; } = 8;

    [Option("-p|--preserve", Description = "When set it preserves timestamps on local files")]
    public bool PreserveTimestamps { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        await ObjectManager.SyncAsync(Source, Destination, Concurrent, PreserveTimestamps).ConfigureAwait(false);
    }
}