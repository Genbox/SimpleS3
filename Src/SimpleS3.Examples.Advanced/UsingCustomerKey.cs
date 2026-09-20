using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.AmazonS3;

// Justification: The standalone example favors a compact walkthrough over IDE style and unused-entry-point inspections.
// ReSharper disable All

namespace Genbox.SimpleS3.Examples.Advanced;

[SuppressMessage("Security", "S4790", Justification = "S3 SSE-C requires the base64 MD5 checksum of the customer key in both upload and download headers.")]
[SuppressMessage("Security", "CA5351", Justification = "S3 SSE-C requires the base64 MD5 checksum of the customer key in both upload and download headers.")]
[SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "These examples dispose downloaded response content with await using.")]
public static class UsingCustomerKey
{
    public static async Task ExampleAsync(CancellationToken token = default)
    {
        using AmazonS3Client client = new AmazonS3Client("YourKeyId", "YourAccessKey", AmazonS3Region.EuWest1);

        byte[] data = "hello world"u8.ToArray();
        byte[] encryptionKey = new byte[32];
        RandomNumberGenerator.Fill(encryptionKey);

        await UsingUploadAsync(client, data, encryptionKey, token);
        await UsingPutObjectAsync(client, data, encryptionKey, token);
    }

    /// <summary>Shows how to use Server Side Encryption with Customer Keys</summary>
    private static async Task UsingUploadAsync(AmazonS3Client client, byte[] data, byte[] encryptionKey, CancellationToken token)
    {
        IUpload upload = client.CreateUpload("bucket-name", "object-name")
                               .WithEncryptionCustomerKey(encryptionKey);

        await upload.UploadDataAsync(data, token);

        IDownload download = client.CreateDownload("bucket-name", "object-name")
                                   .WithEncryptionCustomerKey(encryptionKey);

        GetObjectResponse response = await download.DownloadAsync(token);
        await using ContentStream content = response.Content;
        byte[] responseData = await content.AsDataAsync(token);
        Console.WriteLine($"Downloaded {responseData.Length} bytes.");
    }

    /// <summary>Shows how to use Server Side Encryption with Customer Keys</summary>
    private static async Task UsingPutObjectAsync(AmazonS3Client client, byte[] data, byte[] encryptionKey, CancellationToken token)
    {
        await using MemoryStream ms = new MemoryStream(data);

        //Upload using multiple concurrent connections and use server-side encryption with our own key.
        await client.PutObjectAsync("bucket-name", "object-name", ms, request =>
        {
            request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            request.SseCustomerKey = encryptionKey;
            request.SseCustomerKeyMd5 = MD5.HashData(encryptionKey);
        }, token);

        //Download using multiple concurrent connections and use server-side encryption with our own key.
        GetObjectResponse response = await client.GetObjectAsync("bucket-name", "object-name", request =>
        {
            request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            request.SseCustomerKey = encryptionKey;
            request.SseCustomerKeyMd5 = MD5.HashData(encryptionKey);
        }, token);
        await using ContentStream content = response.Content;
        byte[] responseData = await content.AsDataAsync(token);
        Console.WriteLine($"Downloaded {responseData.Length} bytes.");
    }
}