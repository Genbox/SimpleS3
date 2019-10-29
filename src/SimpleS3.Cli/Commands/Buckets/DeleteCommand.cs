using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets
{
    [Command(Description = "Delete all the objects in a bucket along with the bucket itself")]
    internal class DeleteCommand : CommandBase<Bucket>
    {
        [Argument(0, Description = "Bucket name")]
        [Required]
        public string BucketName { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            await Manager.BucketManager.DeleteAsync(BucketName).ConfigureAwait(false);
            Console.WriteLine("Successfully emptied " + BucketName);
        }
    }
}