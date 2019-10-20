using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects
{
    [Command("mv", Description = "Get the contents of an object")]
    internal class MoveCommand : CommandBase<Program>
    {
        [Argument(0)]
        [Required]
        public string BucketName { get; set; }

        [Argument(1)]
        [Required]
        public string ObjectName { get; set; }

        [Argument(2)]
        [Required]
        public string FileName { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            //GetObjectResponse resp = await ExecuteRequestAsync(client => client.GetObjectAsync(BucketName, ObjectName)).ConfigureAwait(false);

            //if (resp == null)
            //    return 1;

            //using (Stream s = resp.Content.AsStream())
            //{
            //    long length = s.Length;

            //    using (FileStream fs = File.OpenWrite(FileName))
            //        await s.CopyToAsync(fs).ConfigureAwait(false);

            //    Console.WriteLine($"Successfully downloaded {length} bytes from {BucketName}/{ObjectName} to {FileName}");
            //}
        }
    }
}