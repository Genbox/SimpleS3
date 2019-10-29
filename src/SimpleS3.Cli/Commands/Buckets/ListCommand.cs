using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Responses.S3Types;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets
{
    [Command(Description = "List all buckets you own")]
    internal class ListCommand : CommandBase<Bucket>
    {
        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            Console.WriteLine("{0,-21}{1}", "Created on", "Name");

            await foreach (S3Bucket bucket in Manager.BucketManager.ListAsync(token))
                Console.WriteLine("{0,-21}{1}", bucket.CreationDate.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), bucket.Name);
        }
    }
}