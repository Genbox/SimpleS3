using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects
{
    [Command("rm", Description = "Deletes an object")]
    internal class RemoveCommand : CommandBase<Program>
    {
        [Argument(0, Description = "The resource you want to control. E.g. s3://bucket/object")]
        [Required]
        public string Resource { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            //If the user 


            //PutObjectResponse resp = await ExecuteRequestAsync(client => client.PutObjectAsync(BucketName, ObjectName, s)).ConfigureAwait(false);

            //if (resp == null)
            //    return 1;

            //Console.WriteLine($"Successfully uploaded {length} bytes from {FileName} to {BucketName}/{ObjectName}");
        }
    }
}