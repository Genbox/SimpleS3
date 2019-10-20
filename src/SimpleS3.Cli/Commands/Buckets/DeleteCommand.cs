using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets
{
    [Command(Description = "Delete a bucket")]
    internal class DeleteCommand : CommandBase<Bucket>
    {
        [Argument(0)]
        [Required]
        public string BucketName { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            //DeleteBucketResponse resp = await ExecuteRequestAsync(client => client.DeleteBucketAsync(BucketName)).ConfigureAwait(false);

            //if (resp == null)
            //    return 1;

            //Console.WriteLine("Successfully deleted " + BucketName);
        }
    }
}