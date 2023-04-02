using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Objects;

[Command("ls", Description = "List the objects in a bucket")]
internal class ListCommand : OnlineCommandBase
{
    [Argument(0, Description = "Path. E.g: s3://mybucket/prefix/")]
    [Required]
    public string Path { get; set; } = null!;

    [Option("-d|--details", Description = "Show detailed output")]
    public bool IncludeDetails { get; set; }

    [Option("-o|--owner", Description = "Show owner information")]
    public bool IncludeOwner { get; set; }

    protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
    {
        await base.ExecuteAsync(app, token);

        IAsyncEnumerator<S3Object> list = ObjectManager.ListAsync(Path, IncludeOwner).GetAsyncEnumerator(token);

        bool hasData = await list.MoveNextAsync().ConfigureAwait(false);

        if (!hasData)
        {
            Console.WriteLine();
            Console.WriteLine("There were no objects.");
        }
        else
        {
            Console.WriteLine();

            if (IncludeDetails)
            {
                if (IncludeOwner)
                    Console.WriteLine("{0,-20}{1,-12}{2,-18}{3,-38}{4,-20}{5}", "Modified on", "Size", "Storage class", "ETag", "Owner", "Name");
                else
                    Console.WriteLine("{0,-20}{1,-12}{2,-18}{3,-38}{4}", "Modified on", "Size", "Storage class", "ETag", "Name");
            }
            else
                Console.WriteLine("{0,-20}{1,-12}{2}", "Modified on", "Size", "Name");

            do
            {
                S3Object obj = list.Current;

                if (IncludeDetails)
                {
                    if (IncludeOwner)
                    {
                        string? ownerInfo = null;

                        if (obj.Owner != null)
                            ownerInfo = obj.Owner.Name;

                        Console.WriteLine("{0,-20}{1,-12}{2,-18}{3,-38}{4,-20}{5}", obj.LastModifiedOn.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.StorageClass, obj.ETag!, ownerInfo, obj.ObjectKey);
                    }
                    else
                        Console.WriteLine("{0,-20}{1,-12}{2,-18}{3,-38}{4}", obj.LastModifiedOn.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.StorageClass, obj.ETag!, obj.ObjectKey);
                }
                else
                    Console.WriteLine("{0,-20}{1,-12}{2}", obj.LastModifiedOn.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.ObjectKey);
            } while (await list.MoveNextAsync().ConfigureAwait(false));
        }
    }
}