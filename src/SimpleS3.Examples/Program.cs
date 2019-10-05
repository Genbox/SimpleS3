using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Responses.Objects;
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
            //using (S3Client client = AmazonClient.Create("YourKeyIdHere", "YourAccessKeyHere"))
            using (S3Client client = MinioPlaygroundClient.Create())
            {
                const string bucketName = "simple-s3-test";
                const string objectName = "some-object";

                //First we create the a bucket named "simple-s3-test". It might already be there, so we ignore if the request was not a success
                await client.PutBucketAsync(bucketName).ConfigureAwait(false);

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
                    }
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