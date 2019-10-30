using System;
using System.Text;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Fluent;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Examples.Clients.Simple;

namespace Genbox.SimpleS3.Examples
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //
            // Go here and login with your account to create an access key: https://console.aws.amazon.com/iam/home?#/security_credentials
            //

            //Uncomment this line if you want to use it against your own bucket
            //using (S3Client client = AmazonDiClientWithProxy.Create("MyKeyId", "MyAccessKey", AwsRegion.UsEast1))
            using (S3Client client = MinioPlaygroundClient.Create())
            {
                const string bucketName = "simple-s3-test";
                const string objectName = "some-object";

                //First we create the a bucket named "simple-s3-test". It might already be there, so we ignore if the request was not a success
                await client.CreateBucketAsync(bucketName).ConfigureAwait(false);

                //Upload and download an object using the normal API
                await UploadDownloadWithNormalApi(client, bucketName, objectName).ConfigureAwait(false);

                //Upload and download an object using the fluent API
                await UploadDownloadWithFluent(client, bucketName, objectName).ConfigureAwait(false);
            }
        }


        private static async Task UploadDownloadWithNormalApi(S3Client client, string bucketName, string objectName)
        {
            Console.WriteLine();
            Console.WriteLine("Using the normal API");

            //Then we upload an object named "some-object" to the bucket. It contains "Hello World" inside it.
            if (await UploadObject(client, bucketName, objectName).ConfigureAwait(false))
            {
                Console.WriteLine("Successfully uploaded the object");

                GetObjectResponse resp = await DownloadObject(client, bucketName, objectName).ConfigureAwait(false);

                //Here we try to download the object again. If successful, we should see it print the content to the screen.
                if (resp.IsSuccess)
                {
                    Console.WriteLine("Successfully downloaded the object");

                    string content = await resp.Content.AsStringAsync().ConfigureAwait(false);
                    Console.WriteLine("The object contained: " + content);

                    //Finally, we clean up after us and remove the object.
                    if (await DeleteObject(client, bucketName, objectName).ConfigureAwait(false))
                        Console.WriteLine("Successfully deleted the object");
                    else
                        Console.WriteLine("Failed deleting the object");
                }
                else
                    Console.WriteLine("Failed downloading object");
            }
            else
                Console.WriteLine("Failed uploading object");
        }

        private static async Task UploadDownloadWithFluent(S3Client client, string bucketName, string objectName)
        {
            Console.WriteLine();
            Console.WriteLine("Using the fluent API");

            //Upload string
            Upload upload = client.Transfer
                .UploadString(bucketName, objectName, "Hello World!", Encoding.UTF8)
                .WithAccessControl(ObjectCannedAcl.PublicReadWrite)
                .WithCacheControl(CacheControlType.NoCache)
                .WithEncryption();

            PutObjectResponse resp = await upload.ExecuteAsync().ConfigureAwait(false);

            if (resp.IsSuccess)
            {
                Console.WriteLine("Successfully uploaded the object");

                //Download string
                Download download = client.Transfer
                    .Download(bucketName, objectName)
                    .WithRange(0, 10); //Adjust this to return only part of the string

                GetObjectResponse resp2 = await download.ExecuteAsync().ConfigureAwait(false);

                if (resp2.IsSuccess)
                {
                    Console.WriteLine("Successfully downloaded the object");
                    Console.WriteLine("The object contained: " + await resp2.Content.AsStringAsync().ConfigureAwait(false));
                }
            }
        }

        private static async Task<bool> UploadObject(S3Client client, string bucketName, string objectName)
        {
            return (await client.PutObjectStringAsync(bucketName, objectName, "Hello World").ConfigureAwait(false)).IsSuccess;
        }

        private static Task<GetObjectResponse> DownloadObject(S3Client client, string bucketName, string objectName)
        {
            return client.GetObjectAsync(bucketName, objectName);
        }

        private static async Task<bool> DeleteObject(S3Client client, string bucketName, string objectName)
        {
            return (await client.DeleteObjectAsync(bucketName, objectName).ConfigureAwait(false)).IsSuccess;
        }
    }
}