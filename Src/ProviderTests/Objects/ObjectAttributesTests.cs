using System.Security.Cryptography;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ObjectAttributesTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task GetObjectAttributesRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateAttributeBucketAsync(provider, client, async bucket =>
    {
        byte[] data = "object attributes"u8.ToArray();
        const string key = "attributes";
        CancellationToken token = TestContext.Current.CancellationToken;
        PutObjectResponse put = await client.PutObjectDataAsync(bucket, key, data, token: token);
        Assert.Equal(200, put.StatusCode);
        GetObjectAttributesResponse read = await client.GetObjectAttributesAsync(bucket, key, [ObjectAttribute.ETag, ObjectAttribute.ObjectSize], token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.NotNull(put.ETag);
        Assert.Equal(put.ETag.Trim('"'), read.ETag?.Trim('"'));
        Assert.Equal(data.Length, read.ObjectSize);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task ChecksumAttribute(S3Provider provider, string _, ISimpleClient client) => await CreateAttributeBucketAsync(provider, client, async bucket =>
    {
        byte[] data = "checksum attribute"u8.ToArray();
        byte[] checksum = SHA256.HashData(data);
        const string key = "attribute-checksum";
        CancellationToken token = TestContext.Current.CancellationToken;
        PutObjectResponse put = await client.PutObjectDataAsync(bucket, key, data, request =>
        {
            request.ChecksumAlgorithm = ChecksumAlgorithm.Sha256;
            request.Checksum = checksum;
        }, token);
        Assert.Equal(200, put.StatusCode);
        GetObjectAttributesResponse read = await client.GetObjectAttributesAsync(bucket, key, [ObjectAttribute.Checksum], token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(ChecksumType.FullObject, read.ChecksumType);
        Assert.Equal(checksum, read.ChecksumSha256);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipartPartsAttribute(S3Provider provider, string _, ISimpleClient client) => await CreateAttributeBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        byte[][] parts = [new byte[5 * 1024 * 1024], "tail"u8.ToArray()];
        CreateMultipartUploadResponse created = await client.CreateMultipartUploadAsync(bucket, "multipart", request => request.ChecksumAlgorithm = ChecksumAlgorithm.Sha256, token);
        Assert.Equal(200, created.StatusCode);
        List<S3PartInfo> completion = [];

        for (int i = 0; i < parts.Length; i++)
        {
            byte[] checksum = SHA256.HashData(parts[i]);
            UploadPartResponse part = await UploadTestPartAsync(client, bucket, "multipart", created.UploadId, i + 1, parts[i], request =>
            {
                request.ChecksumAlgorithm = ChecksumAlgorithm.Sha256;
                request.Checksum = checksum;
            });
            completion.Add(new S3PartInfo(part.ETag, i + 1, checksum, ChecksumAlgorithm.Sha256));
        }

        Assert.Equal(200, (await client.CompleteMultipartUploadAsync(bucket, "multipart", created.UploadId, completion, token: token)).StatusCode);
        GetObjectAttributesResponse read = await client.GetObjectAttributesAsync(bucket, "multipart", [ObjectAttribute.ObjectParts], token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(2, read.TotalPartsCount);
        Assert.False(read.IsTruncated);
        Assert.Equal(2, read.Parts.Count);

        for (int i = 0; i < parts.Length; i++)
        {
            S3ObjectAttributePart part = Assert.Single(read.Parts, item => item.PartNumber == i + 1);
            Assert.Equal(parts[i].Length, part.Size);
            Assert.Equal(SHA256.HashData(parts[i]), part.ChecksumSha256);
        }
    });

    private static Task CreateAttributeBucketAsync(S3Provider provider, ISimpleClient client, Func<string, Task> action) => CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        Assert.Equal(200, (await client.PutPublicAccessBlockAsync(bucket, request =>
        {
            request.BlockPublicPolicy = false;
            request.RestrictPublicBuckets = false;
        }, token)).StatusCode);
        string policy = $$"""
                          {"Version":"2012-10-17","Statement":[{"Effect":"Allow","Principal":"*","Action":["s3:GetObject","s3:GetObjectAttributes"],"Resource":"arn:aws:s3:::{{bucket}}/*"}]}
                          """;
        Assert.Equal(204, (await client.PutBucketPolicyAsync(bucket, policy, token: token)).StatusCode);
        await action(bucket);
    });
}