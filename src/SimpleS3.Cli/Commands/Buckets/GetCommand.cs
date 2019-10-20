using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets
{
    [Command(Description = "Get the contents of a bucket")]
    internal class GetCommand : CommandBase<Bucket>
    {
        [Argument(0)]
        [Required]
        public string BucketName { get; set; }

        [Option("-d")]
        public bool IncludeDetails { get; set; }

        [Option("-o")]
        public bool IncludeOwner { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            //IAsyncEnumerator<S3Object> list = ExecuteAsyncEnumerable(client => client.ListObjectsRecursiveAsync(BucketName, IncludeOwner)).GetAsyncEnumerator();

            //if (list == null)
            //    return 1;

            //bool hasData = await list.MoveNextAsync().ConfigureAwait(false);

            //if (!hasData)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("There were no objects.");
            //}
            //else
            //{
            //    Console.WriteLine();

            //    if (IncludeDetails)
            //    {
            //        if (IncludeOwner)
            //            Console.WriteLine("{0,-20}{1,-12}{2,-15}{3,-38}{4,-10}{5}", "Modified on", "Size", "Storage class", "ETag", "Owner", "Name");
            //        else
            //            Console.WriteLine("{0,-20}{1,-12}{2,-15}{3,-38}{4}", "Modified on", "Size", "Storage class", "ETag", "Name");

            //    }
            //    else
            //        Console.WriteLine("{0,-20}{1,-12}{2}", "Modified on", "Size", "Name");

            //    do
            //    {
            //        S3Object obj = list.Current;

            //        if (IncludeDetails)
            //        {
            //            if (IncludeOwner)
            //            {
            //                string ownerInfo = string.Empty;

            //                if (obj.Owner != null)
            //                    ownerInfo = obj.Owner.Name;

            //                Console.WriteLine("{0,-20}{1,-12}{2,-15}{3,-38}{4,-20}{5,-20}", obj.LastModified.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.StorageClass, obj.ETag, ownerInfo, obj.Name);
            //            }
            //            else
            //            {
            //                Console.WriteLine("{0,-20}{1,-12}{2,-15}{3,-38}{4}", obj.LastModified.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.StorageClass, obj.ETag, obj.Name);
            //            }
            //        }
            //        else
            //            Console.WriteLine("{0,-20}{1,-12}{2}", obj.LastModified.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.Name);

            //    } while (await list.MoveNextAsync().ConfigureAwait(false));
            //}
        }
    }
}