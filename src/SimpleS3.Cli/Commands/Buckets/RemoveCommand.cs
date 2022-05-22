using System.ComponentModel.DataAnnotations;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets;

[Command("remove", Description = "Delete all the objects in a bucket along with the bucket itself")]
internal class RemoveCommand : OnlineCommandBase
{
    [Argument(0, Description = "Bucket name")]
    [Required]
    public string BucketName { get; set; } = null!;

    [Option("-f|--force", Description = "Force delete the bucket")]
    public bool Force { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        bool hasError = false;

        await foreach (S3DeleteError error in BucketManager.DeleteAsync(BucketName, Force).WithCancellation(token))
        {
            hasError = true;
            Console.WriteLine("Failed to delete " + error.ObjectKey);
        }

        if (!hasError)
            Console.WriteLine("Successfully deleted " + BucketName);
    }
}