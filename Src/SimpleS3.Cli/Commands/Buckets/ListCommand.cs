using System.Globalization;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets;

[Command("list", Description = "List all buckets you own")]
internal class ListCommand : OnlineCommandBase
{
    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        Console.WriteLine("{0,-21}{1}", "Created on", "Name");

        await foreach (S3Bucket bucket in BucketManager.ListAsync(token))
            Console.WriteLine("{0,-21}{1}", bucket.CreatedOn.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), bucket.BucketName);
    }
}