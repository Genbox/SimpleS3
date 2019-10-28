using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects
{
    [Command("rm", Description = "Deletes an object")]
    internal class RemoveCommand : CommandBase<S3Cli>
    {
        [Argument(0, Description = "The object you want to delete. E.g. s3://bucket/object")]
        [Required]
        public string Resource { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            await Manager.ObjectManager.DeleteAsync(Resource).ConfigureAwait(false);

            Console.WriteLine($"Successfully deleted {Resource}");
        }
    }
}