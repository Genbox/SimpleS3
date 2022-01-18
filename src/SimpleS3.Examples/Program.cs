using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.AmazonS3;

namespace Genbox.SimpleS3.Examples;

internal class Program
{
    private static async Task Main(string[] args)
    {
        //
        // Go here and login with your account to create an access key: https://console.aws.amazon.com/iam/home?#/security_credentials
        //

        using (AmazonS3Client client = new AmazonS3Client("MyKeyId", "MyAccessKey", AmazonS3Region.UsEast1))
        {
            //We add a unique identifier to the bucket name, as they have to be unique across ALL of AWS S3.
            string bucketName = "simple-s3-test-" + Guid.NewGuid();
            const string objectName = "some-object";

            //First we create the a bucket.
            await client.CreateBucketAsync(bucketName);

            //Upload then download an object using the normal API.
            await UploadDownloadStandard(client, bucketName, objectName);

            //Upload then download an object using the Transfer API.
            await UploadDownloadTransfer(client, bucketName, objectName);
        }
    }

    private static async Task UploadDownloadStandard(AmazonS3Client client, string bucketName, string objectName)
    {
        Console.WriteLine();
        Console.WriteLine("Using the standard API");

        //We upload and object to the bucket with "Hello World" inside it.
        await using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello World"));
        PutObjectResponse putResp = await client.PutObjectAsync(bucketName, objectName, memoryStream);

        if (putResp.IsSuccess)
        {
            Console.WriteLine("Successfully uploaded the object");

            GetObjectResponse getResp = await client.GetObjectAsync(bucketName, objectName);

            //Here we try to download the object again. If successful, we should see it print the content to the screen.
            if (getResp.IsSuccess)
            {
                Console.WriteLine("Success! The object contained: " + await getResp.Content.AsStringAsync());

                //Finally, we clean up after us and remove the object.
                DeleteObjectResponse delResp = await client.DeleteObjectAsync(bucketName, objectName);

                Console.WriteLine(delResp.IsSuccess ? "Successfully deleted the object" : "Failed deleting the object");
            }
            else
                Console.WriteLine("Failed downloading object");
        }
        else
            Console.WriteLine("Failed uploading object");
    }

    private static async Task UploadDownloadTransfer(AmazonS3Client client, string bucketName, string objectName)
    {
        Console.WriteLine();
        Console.WriteLine("Using the Transfer API");

        //The Transfer API is an easy-to-use API for building requests.
        IUpload upload = client.CreateUpload(bucketName, objectName)
            .WithAccessControl(ObjectCannedAcl.PublicReadWrite)
            .WithCacheControl(CacheControlType.NoCache)
            .WithEncryption();

        PutObjectResponse putResp = await upload.UploadStringAsync("Hello World!");

        if (putResp.IsSuccess)
        {
            Console.WriteLine("Successfully uploaded the object");

            //Download string
            IDownload download = client.CreateDownload(bucketName, objectName)
                .WithRange(0, 5); //Adjust this to return only part of the string

            GetObjectResponse getResp = await download.DownloadAsync();

            if (getResp.IsSuccess)
                Console.WriteLine("Success! The object contained: " + await getResp.Content.AsStringAsync());
            else
                Console.WriteLine("Failed to download the object");
        }
        else
            Console.WriteLine("Failed to upload the object");
    }
}