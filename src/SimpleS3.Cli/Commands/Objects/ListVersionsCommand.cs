using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects;

[Command("lsv", Description = "List the object versions in a bucket")]
internal class ListVersionsCommand : OnlineCommandBase
{
    [Argument(0, Description = "Path. E.g. s3://mybucket/prefix/")]
    [Required]
    public string Path { get; set; } = null!;

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        IAsyncEnumerator<S3Version> list = ObjectManager.ListVersionsAsync(Path).GetAsyncEnumerator(token);

        bool hasData = await list.MoveNextAsync().ConfigureAwait(false);

        if (!hasData)
        {
            Console.WriteLine();
            Console.WriteLine("There were no object versions.");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("{0,-20}{1,-12}{2,-18}{3,-38}{4,-20}{5,-10}{6}", "Modified on", "Size", "Storage class", "ETag", "Owner", "Is latest", "Name");

            do
            {
                S3Version obj = list.Current;

                string? ownerInfo = null;

                if (obj.Owner != null)
                    ownerInfo = obj.Owner.Name;

                Console.WriteLine("{0,-20}{1,-12}{2,-18}{3,-38}{4,-20}{5,-10}{6}", obj.LastModified.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.StorageClass, obj.Etag, ownerInfo, obj.IsLatest, obj.ObjectKey);
            } while (await list.MoveNextAsync().ConfigureAwait(false));
        }
    }
}