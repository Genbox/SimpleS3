using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Responses.S3Types;
using McMaster.Extensions.CommandLineUtils;

namespace Genbox.SimpleS3.Cli.Commands.Buckets
{
    [Command(Description = "List the objects in a bucket")]
    internal class GetCommand : CommandBase<Bucket>
    {
        [Argument(0)]
        [Required]
        public string BucketName { get; set; }

        [Option("--include-details|-d", Description = "Show detailed output")]
        public bool IncludeDetails { get; set; }

        [Option("--include-owner|-o", Description = "Include owner information")]
        public bool IncludeOwner { get; set; }

        protected override async Task ExecuteAsync(CommandLineApplication app, CancellationToken token)
        {
            IAsyncEnumerator<S3Object> list = Manager.BucketManager.GetAsync(BucketName, IncludeOwner).GetAsyncEnumerator(token);

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
                            string ownerInfo = string.Empty;

                            if (obj.Owner != null)
                                ownerInfo = obj.Owner.Name;

                            Console.WriteLine("{0,-20}{1,-12}{2,-18}{3,-38}{4,-20}{5}", obj.LastModified.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.StorageClass, obj.ETag, ownerInfo, obj.ObjectKey);
                        }
                        else
                            Console.WriteLine("{0,-20}{1,-12}{2,-18}{3,-38}{4}", obj.LastModified.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.StorageClass, obj.ETag, obj.ObjectKey);
                    }
                    else
                        Console.WriteLine("{0,-20}{1,-12}{2}", obj.LastModified.ToString("yyy-MM-dd hh:mm:ss", DateTimeFormatInfo.InvariantInfo), obj.Size, obj.ObjectKey);
                } while (await list.MoveNextAsync().ConfigureAwait(false));
            }
        }
    }
}