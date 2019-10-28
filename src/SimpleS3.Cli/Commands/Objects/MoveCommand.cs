using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects
{
    [Command("mv", Description = "Move an object")]
    internal class MoveCommand : ObjectOperationBase
    {
        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            await Manager.ObjectManager.CopyAsync(Source, Destination).ConfigureAwait(false);

            Console.WriteLine($"Successfully moved {Source} to {Destination}");
        }
    }
}