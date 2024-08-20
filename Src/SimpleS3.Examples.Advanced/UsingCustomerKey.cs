using System.Security.Cryptography;
using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.AmazonS3;

// ReSharper disable All

namespace Genbox.SimpleS3.Examples.Advanced;

public static class UsingCustomerKey
{
    public static async Task Example()
    {
        using AmazonS3Client client = new AmazonS3Client("YourKeyId", "YourAccessKey", AmazonS3Region.EuWest1);

        byte[] data = "hello world"u8.ToArray();
        byte[] encryptionKey = new byte[32];
        Random.Shared.NextBytes(encryptionKey);

        await UsingUpload(client, data, encryptionKey);
        await UsingPutObject(client, data, encryptionKey);
    }

    /// <summary>Shows how to use Server Side Encryption with Customer Keys</summary>
    private static async Task UsingUpload(AmazonS3Client client, byte[] data, byte[] encryptionKey)
    {
        IUpload upload = client.CreateUpload("bucket-name", "object-name")
                               .WithEncryptionCustomerKey(encryptionKey);

        await upload.UploadDataAsync(data);

        IDownload download = client.CreateDownload("bucket-name", "object-name")
                                   .WithEncryptionCustomerKey(encryptionKey);

        GetObjectResponse response = await download.DownloadAsync();
        byte[] responseData = await response.Content.AsDataAsync();
    }

    /// <summary>Shows how to use Server Side Encryption with Customer Keys</summary>
    private static async Task UsingPutObject(AmazonS3Client client, byte[] data, byte[] encryptionKey)
    {
        using MemoryStream ms = new MemoryStream(data);

        //Upload using multiple concurrent connections and use server-side encryption with our own key.
        await client.PutObjectAsync("bucket-name", "object-name", ms, request =>
        {
            request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            request.SseCustomerKey = encryptionKey;
            request.SseCustomerKeyMd5 = MD5.HashData(encryptionKey);
        });

        //Download using multiple concurrent connections and use server-side encryption with our own key.
        GetObjectResponse response = await client.GetObjectAsync("bucket-name", "object-name", request =>
        {
            request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            request.SseCustomerKey = encryptionKey;
            request.SseCustomerKeyMd5 = MD5.HashData(encryptionKey);
        });

        byte[] responseData = await response.Content.AsDataAsync();
    }
}