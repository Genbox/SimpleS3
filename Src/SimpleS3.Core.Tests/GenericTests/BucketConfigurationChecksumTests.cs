using System.Security.Cryptography;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class BucketConfigurationChecksumTests
{
    [Fact]
    public async Task AccelerationChecksumMatchesSerializedBody()
    {
        PutBucketAccelerateConfigurationRequest request = new PutBucketAccelerateConfigurationRequest("bucket", true)
        {
            ChecksumAlgorithm = ChecksumAlgorithm.Sha256
        };
        await using Stream body = new PutBucketAccelerateConfigurationRequestMarshal().MarshalRequest(request, new SimpleS3Config());

        Assert.Equal(Convert.ToBase64String(SHA256.HashData(await ReadAsync(body))), request.Headers["x-amz-checksum-sha256"]);
    }

    [Fact]
    public async Task CorsChecksumMatchesSerializedBody()
    {
        PutBucketCorsRequest request = new PutBucketCorsRequest("bucket", [new S3CorsRule()])
        {
            ChecksumAlgorithm = ChecksumAlgorithm.Sha256
        };
        await using Stream body = new PutBucketCorsRequestMarshal().MarshalRequest(request, new SimpleS3Config());

        Assert.Equal(Convert.ToBase64String(SHA256.HashData(await ReadAsync(body))), request.Headers["x-amz-checksum-sha256"]);
    }

    private static async Task<byte[]> ReadAsync(Stream stream)
    {
        await using MemoryStream copy = new MemoryStream();
        await stream.CopyToAsync(copy, TestContext.Current.CancellationToken);
        return copy.ToArray();
    }
}