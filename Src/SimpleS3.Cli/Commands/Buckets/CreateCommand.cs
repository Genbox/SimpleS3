using System.ComponentModel.DataAnnotations;
using Genbox.SimpleS3.Core.Enums;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets;

[Command("create", Description = "Create a bucket")]
internal class CreateCommand : OnlineCommandBase
{
    [Argument(0, Description = "Bucket name")]
    [Required]
    public string BucketName { get; set; } = null!;

    [Option("-e|--enable-locking", Description = "Enable object locking support on the bucket")]
    public bool EnableLocking { get; set; }

    [Option("-c|--canned-acl", Description = "Set the a canned ACL on the bucket")]
    public BucketCannedAcl CannedAcl { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        await BucketManager.CreateAsync(BucketName, EnableLocking, CannedAcl).ConfigureAwait(false);
        Console.WriteLine("Successfully created " + BucketName);
    }
}