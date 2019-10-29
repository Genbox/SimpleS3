using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets
{
    [Command("bucket")]
    [Subcommand(typeof(CreateCommand), typeof(DeleteCommand), typeof(EmptyCommand), typeof(GetCommand), typeof(ListCommand))]
    internal class Bucket : CommandBase<S3Cli>
    {
        protected override Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            app.ShowHelp();
            return Task.CompletedTask;
        }
    }
}