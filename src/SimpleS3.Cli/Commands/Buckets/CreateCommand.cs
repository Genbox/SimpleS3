using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets
{
    [Command(Description = "Create a bucket")]
    internal class CreateCommand : CommandBase<Bucket>
    {
        [Argument(0)]
        [Required]
        public string BucketName { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            await Manager.BucketManager.CreateAsync(BucketName).ConfigureAwait(false);
            Console.WriteLine("Successfully created " + BucketName);
        }
    }
}