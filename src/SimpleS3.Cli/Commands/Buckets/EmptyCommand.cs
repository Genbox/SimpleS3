using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets
{
    [Command(Description = "Delete all objects within a bucket")]
    internal class EmptyCommand : CommandBase<Bucket>
    {
        [Argument(0, Description = "Bucket name")]
        [Required]
        public string BucketName { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            await Manager.BucketManager.EmptyAsync(BucketName).ConfigureAwait(false);
            Console.WriteLine("Successfully emptied " + BucketName);
        }
    }
}