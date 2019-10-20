using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects
{
    [Command("cp", Description = "Upload the contents of an object")]
    internal class CopyCommand : CommandBase<Program>
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
            //using (Stream s = File.OpenRead(FileName))
            //{
            //    long length = s.Length;

            //    PutObjectResponse resp = await ExecuteRequestAsync(client => client.PutObjectAsync(BucketName, ObjectName, s)).ConfigureAwait(false);

            //    if (resp == null)
            //        return 1;

            //    Console.WriteLine($"Successfully uploaded {length} bytes from {FileName} to {BucketName}/{ObjectName}");
            //}
        }
    }
}