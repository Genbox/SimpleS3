using System.Security.Cryptography;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ListingChecksumMetadataTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task ChecksumOverwritePreservesVersionMetadata(S3Provider provider, string bucket, ISimpleClient client)
        => await ChecksumOverwritePreservesVersionMetadataCoreAsync(provider, bucket, client);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task Sha1OverwritePreservesVersionMetadata(S3Provider provider, string bucket, ISimpleClient client) => await AssertChecksumOverwritePreservesVersionMetadataAsync(provider,
        bucket,
        client,
        ChecksumAlgorithm.Sha1,
        ChecksumAlgorithm.Sha256);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task OrdinaryAndMultipartOverwritesPreserveBothChecksumVersions(S3Provider provider, string bucket, ISimpleClient client)
    {
        await AssertChecksumTransitionAsync(provider, bucket, client, true);
        await AssertChecksumTransitionAsync(provider, bucket, client, false);
    }

    private static async Task AssertChecksumTransitionAsync(S3Provider provider, string bucket, ISimpleClient client, bool multipartFirst)
    {
        const ChecksumAlgorithm multipartAlgorithm = ChecksumAlgorithm.Crc32C;
        const ChecksumAlgorithm ordinaryAlgorithm = ChecksumAlgorithm.Sha256;
        byte[][] multipartParts = multipartFirst
            ? [new byte[5 * 1024 * 1024], "multipart-version-tail"u8.ToArray()]
            : [new byte[5 * 1024 * 1024], new byte[5 * 1024 * 1024], "multipart-version-tail"u8.ToArray()];
        for (int index = 0; index < multipartParts.Length - 1; index++)
            Array.Fill(multipartParts[index], (byte)(0x3d + index));
        byte[] multipartData = multipartParts.SelectMany(part => part).ToArray();
        byte[][] partChecksums = multipartParts.Select(part => ComputeChecksum(multipartAlgorithm, part)).ToArray();
        byte[] multipartChecksum = ComputeChecksum(multipartAlgorithm, multipartData);
        byte[] ordinaryData = "ordinary-checksum-version"u8.ToArray();
        byte[] ordinaryChecksum = ComputeChecksum(ordinaryAlgorithm, ordinaryData);
        string prefix = "checksum-transition-" + (multipartFirst ? "multipart-first-" : "ordinary-first-");
        string key = prefix + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        string? uploadId = null;
        HashSet<string> versions = new HashSet<string>(StringComparer.Ordinal);
        bool completed = false;

        async Task<string> PutOrdinaryVersionAsync()
        {
            PutObjectResponse ordinary = await client.PutObjectDataAsync(bucket, key, ordinaryData, request =>
            {
                request.ChecksumAlgorithm = ordinaryAlgorithm;
                request.Checksum = ordinaryChecksum;
                request.Metadata.Add("audit", "ordinary");
            }, token);
            Assert.True(ordinary.IsSuccess, $"Ordinary PUT: {ordinary.StatusCode} {ordinary.Error?.Code}");
            Assert.False(string.IsNullOrWhiteSpace(ordinary.VersionId));
            versions.Add(ordinary.VersionId);
            return ordinary.VersionId;
        }

        async Task<string> PutMultipartVersionAsync()
        {
            CreateMultipartUploadResponse initiated = await client.CreateMultipartUploadAsync(bucket, key, request =>
            {
                request.Metadata.Add("audit", "multipart");
                request.ChecksumAlgorithm = multipartAlgorithm;
                request.ChecksumType = ChecksumType.FullObject;
            }, token);
            Assert.True(initiated.IsSuccess, $"Initiate: {initiated.StatusCode} {initiated.Error?.Code}");
            uploadId = initiated.UploadId;
            Assert.NotNull(uploadId);

            List<S3PartInfo> completionParts = new List<S3PartInfo>(multipartParts.Length);

            for (int index = 0; index < multipartParts.Length; index++)
            {
                await using MemoryStream stream = new MemoryStream(multipartParts[index]);
                UploadPartResponse uploaded = await client.UploadPartAsync(bucket, key, index + 1, uploadId, stream, request =>
                {
                    request.ChecksumAlgorithm = multipartAlgorithm;
                    request.Checksum = partChecksums[index];
                }, token);
                Assert.True(uploaded.IsSuccess, $"Part {index + 1}: {uploaded.StatusCode} {uploaded.Error?.Code}");
                Assert.NotNull(uploaded.ETag);
                completionParts.Add(new S3PartInfo(uploaded.ETag, index + 1, partChecksums[index], multipartAlgorithm));
            }

            CompleteMultipartUploadResponse multipart = await client.CompleteMultipartUploadAsync(bucket, key, uploadId, completionParts, request =>
            {
                request.ChecksumAlgorithm = multipartAlgorithm;
                request.ChecksumType = ChecksumType.FullObject;
                request.Checksum = multipartChecksum;
                request.MpuObjectSize = multipartData.Length;
            }, token);
            Assert.True(multipart.IsSuccess, $"Complete: {multipart.StatusCode} {multipart.Error?.Code}");
            completed = true;
            Assert.False(string.IsNullOrWhiteSpace(multipart.VersionId));
            versions.Add(multipart.VersionId);
            return multipart.VersionId;
        }

        try
        {
            string firstVersion;
            string secondVersion;

            if (multipartFirst)
            {
                firstVersion = await PutMultipartVersionAsync();
                if (provider == S3Provider.BackBlazeB2)
                    await Task.Delay(TimeSpan.FromSeconds(1.1), token);
                secondVersion = await PutOrdinaryVersionAsync();
            }
            else
            {
                firstVersion = await PutOrdinaryVersionAsync();
                if (provider == S3Provider.BackBlazeB2)
                    await Task.Delay(TimeSpan.FromSeconds(1.1), token);
                secondVersion = await PutMultipartVersionAsync();
            }

            Assert.NotEqual(firstVersion, secondVersion, StringComparer.Ordinal);
            string multipartVersion = multipartFirst ? firstVersion : secondVersion;
            string ordinaryVersion = multipartFirst ? secondVersion : firstVersion;

            if (provider == S3Provider.BackBlazeB2)
                await Task.Delay(TimeSpan.FromSeconds(1), token);

            ChecksumAlgorithm ordinaryReadAlgorithm = ordinaryAlgorithm;
            byte[] ordinaryReadChecksum = ComputeChecksum(ordinaryReadAlgorithm, ordinaryData);
            await AssertReadAsync(client, bucket, key, null,
                multipartFirst ? ordinaryData : multipartData,
                multipartFirst ? ordinaryReadAlgorithm : multipartAlgorithm,
                multipartFirst ? ordinaryReadChecksum : multipartChecksum,
                token, multipartFirst ? "ordinary" : "multipart");
            await AssertReadAsync(client, bucket, key, multipartVersion, multipartData, multipartAlgorithm, multipartChecksum, token, "multipart");
            await AssertReadAsync(client, bucket, key, ordinaryVersion, ordinaryData, ordinaryReadAlgorithm, ordinaryReadChecksum, token, "ordinary");

            ListObjectsResponse objects = await client.ListObjectsAsync(bucket, request => request.Prefix = key, token);
            ListObjectVersionsResponse listedVersions = await client.ListObjectVersionsAsync(bucket, request => request.Prefix = key, token);
            Assert.True(objects.IsSuccess, $"ListObjects: {objects.StatusCode} {objects.Error?.Code}");
            Assert.True(listedVersions.IsSuccess, $"ListObjectVersions: {listedVersions.StatusCode} {listedVersions.Error?.Code}");
            S3Object current = Assert.Single(objects.Objects, item => item.ObjectKey == key);
            S3Version multipartListed = Assert.Single(listedVersions.Versions, item => item.ObjectKey == key && item.VersionId == multipartVersion);
            S3Version ordinaryListed = Assert.Single(listedVersions.Versions, item => item.ObjectKey == key && item.VersionId == ordinaryVersion);
            ChecksumAlgorithm listedOrdinaryAlgorithm = ordinaryAlgorithm;
            ChecksumAlgorithm listedMultipartAlgorithm = multipartAlgorithm;
            ChecksumType ordinaryListedType = provider is S3Provider.AmazonS3 or S3Provider.BackBlazeB2 ? ChecksumType.FullObject : ChecksumType.Unknown;
            ChecksumType multipartListedType = provider is S3Provider.AmazonS3 or S3Provider.BackBlazeB2 ? ChecksumType.FullObject : ChecksumType.Unknown;
            Assert.Multiple(() => Assert.Contains(multipartFirst ? listedOrdinaryAlgorithm : listedMultipartAlgorithm, current.ChecksumAlgorithms),
                () => Assert.Equal(multipartFirst ? ordinaryListedType : multipartListedType, current.ChecksumType),
                () => Assert.Contains(listedMultipartAlgorithm, multipartListed.ChecksumAlgorithms),
                () => Assert.Equal(multipartListedType, multipartListed.ChecksumType),
                () => Assert.Equal(!multipartFirst, multipartListed.IsLatest),
                () => Assert.Contains(listedOrdinaryAlgorithm, ordinaryListed.ChecksumAlgorithms),
                () => Assert.Equal(ordinaryListedType, ordinaryListed.ChecksumType),
                () => Assert.Equal(multipartFirst, ordinaryListed.IsLatest));
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            if (!completed && uploadId != null)
                Assert.Equal(204, (await client.AbortMultipartUploadAsync(bucket, key, uploadId, token: cleanup.Token)).StatusCode);
            foreach (string version in versions)
                Assert.Contains((await client.DeleteObjectAsync(bucket, key, request => request.VersionId = version, cleanup.Token)).StatusCode, new[] { 204, 404 });
            if (uploadId != null)
                Assert.Equal(404, (await client.ListPartsAsync(bucket, key, uploadId, token: cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, key, token: cleanup.Token)).StatusCode);
            ListMultipartUploadsResponse pending = await client.ListMultipartUploadsAsync(bucket, request => request.Prefix = prefix, cleanup.Token);
            Assert.True(pending.IsSuccess);
            Assert.DoesNotContain(pending.Uploads, upload => upload.ObjectKey == key);
        }
    }

    private static async Task ChecksumOverwritePreservesVersionMetadataCoreAsync(S3Provider provider, string bucket, ISimpleClient client)
    {
        const ChecksumAlgorithm firstAlgorithm = ChecksumAlgorithm.Crc32;
        const ChecksumAlgorithm secondAlgorithm = ChecksumAlgorithm.Crc64Nvme;
        await AssertChecksumOverwritePreservesVersionMetadataAsync(provider,
            bucket,
            client,
            firstAlgorithm,
            secondAlgorithm);
    }

    private static async Task AssertChecksumOverwritePreservesVersionMetadataAsync(S3Provider provider,
                                                                                   string bucket,
                                                                                   ISimpleClient client,
                                                                                   ChecksumAlgorithm firstAlgorithm,
                                                                                   ChecksumAlgorithm secondAlgorithm)
    {
        byte[] firstData = "checksum-overwrite-first"u8.ToArray();
        byte[] secondData = "checksum-overwrite-second-longer"u8.ToArray();
        byte[] firstChecksum = ComputeChecksum(firstAlgorithm, firstData);
        byte[] secondChecksum = ComputeChecksum(secondAlgorithm, secondData);
        string key = "checksum-overwrite-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        HashSet<string> versions = new HashSet<string>(StringComparer.Ordinal);

        try
        {
            PutObjectResponse first = await client.PutObjectDataAsync(bucket, key, firstData, request =>
            {
                request.ChecksumAlgorithm = firstAlgorithm;
                request.Checksum = firstChecksum;
            }, token);
            Assert.True(first.IsSuccess, $"First PUT: {first.StatusCode} {first.Error?.Code}");
            Assert.False(string.IsNullOrWhiteSpace(first.VersionId));
            versions.Add(first.VersionId);

            PutObjectResponse second = await client.PutObjectDataAsync(bucket, key, secondData, request =>
            {
                request.ChecksumAlgorithm = secondAlgorithm;
                request.Checksum = secondChecksum;
            }, token);
            Assert.True(second.IsSuccess, $"Second PUT: {second.StatusCode} {second.Error?.Code}");
            Assert.False(string.IsNullOrWhiteSpace(second.VersionId));
            Assert.NotEqual(first.VersionId, second.VersionId, StringComparer.Ordinal);
            versions.Add(second.VersionId);

            if (provider == S3Provider.BackBlazeB2)
                await Task.Delay(TimeSpan.FromSeconds(1), token);

            await AssertReadAsync(client, bucket, key, null, secondData, secondAlgorithm, secondChecksum, token);
            await AssertReadAsync(client, bucket, key, first.VersionId, firstData, firstAlgorithm, firstChecksum, token);
            await AssertReadAsync(client, bucket, key, second.VersionId, secondData, secondAlgorithm, secondChecksum, token);

            ListObjectsResponse objects = await client.ListObjectsAsync(bucket, request => request.Prefix = key, token);
            ListObjectVersionsResponse listedVersions = await client.ListObjectVersionsAsync(bucket, request => request.Prefix = key, token);
            Assert.True(objects.IsSuccess, $"ListObjects: {objects.StatusCode} {objects.Error?.Code}");
            Assert.True(listedVersions.IsSuccess, $"ListObjectVersions: {listedVersions.StatusCode} {listedVersions.Error?.Code}");
            S3Object current = Assert.Single(objects.Objects, item => item.ObjectKey == key);
            S3Version firstListed = Assert.Single(listedVersions.Versions, item => item.ObjectKey == key && item.VersionId == first.VersionId);
            S3Version secondListed = Assert.Single(listedVersions.Versions, item => item.ObjectKey == key && item.VersionId == second.VersionId);
            ChecksumAlgorithm firstListedAlgorithm = firstAlgorithm;
            ChecksumAlgorithm secondListedAlgorithm = secondAlgorithm;
            ChecksumType listedType = provider is S3Provider.AmazonS3 or S3Provider.BackBlazeB2 ? ChecksumType.FullObject : ChecksumType.Unknown;
            Assert.Multiple(() => Assert.Contains(secondListedAlgorithm, current.ChecksumAlgorithms),
                () => Assert.Equal(listedType, current.ChecksumType),
                () => Assert.Contains(firstListedAlgorithm, firstListed.ChecksumAlgorithms),
                () => Assert.Equal(listedType, firstListed.ChecksumType),
                () => Assert.False(firstListed.IsLatest),
                () => Assert.Contains(secondListedAlgorithm, secondListed.ChecksumAlgorithms),
                () => Assert.Equal(listedType, secondListed.ChecksumType),
                () => Assert.True(secondListed.IsLatest));
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            foreach (string version in versions)
                Assert.Equal(204, (await client.DeleteObjectAsync(bucket, key, request => request.VersionId = version, cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, key, token: cleanup.Token)).StatusCode);
        }
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi, ChecksumAlgorithm.Crc32, ChecksumAlgorithm.Crc32C, ChecksumAlgorithm.Crc64Nvme, ChecksumAlgorithm.Sha1)]
    public async Task OrdinaryChecksumMetadataInObjectListings(S3Provider provider, string bucket, ISimpleClient client, ChecksumAlgorithm algorithm)
    {
        byte[] data = "listing-checksum-metadata"u8.ToArray();
        using HashAlgorithm hash = ChecksumHashAlgorithm.Create(algorithm);
        byte[] checksum = hash.ComputeHash(data);
        string key = "listing-checksum-metadata-" + algorithm.ToString().ToLowerInvariant() + "-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        string? version = null;
        bool created = false;

        try
        {
            PutObjectResponse put;

            // Backblaze occasionally returns HTTP 500 for checksummed PUTs; retry the same key and payload so a transient upload error does not mask the listing assertion.
            for (int attempt = 0;; attempt++)
            {
                put = await client.PutObjectDataAsync(bucket, key, data, request =>
                {
                    request.ChecksumAlgorithm = algorithm;
                    request.Checksum = checksum;
                }, token);

                if (provider != S3Provider.BackBlazeB2 || put.StatusCode != 500 || attempt == 2)
                    break;

                await Task.Delay(TimeSpan.FromSeconds(1 << attempt), token);
            }

            Assert.True(put.IsSuccess, $"PUT: {put.StatusCode} {put.Error?.Code}");
            created = true;
            version = put.VersionId;

            if (provider == S3Provider.BackBlazeB2)
                await Task.Delay(TimeSpan.FromSeconds(1), token);

            ListObjectsResponse objects = await client.ListObjectsAsync(bucket, request => request.Prefix = key, token);
            ListObjectVersionsResponse versions = await client.ListObjectVersionsAsync(bucket, request => request.Prefix = key, token);
            Assert.True(objects.IsSuccess, $"ListObjects: {objects.StatusCode} {objects.Error?.Code}");
            Assert.True(versions.IsSuccess, $"ListObjectVersions: {versions.StatusCode} {versions.Error?.Code}");
            S3Object listedObject = Assert.Single(objects.Objects, item => item.ObjectKey == key);
            S3Version listedVersion = Assert.Single(versions.Versions, item => item.ObjectKey == key && (version == null || item.VersionId == version));
            ChecksumAlgorithm expectedAlgorithm = algorithm;
            ChecksumType expectedType = provider is S3Provider.AmazonS3 or S3Provider.BackBlazeB2 ? ChecksumType.FullObject : ChecksumType.Unknown;
            Assert.Multiple(() => Assert.Contains(expectedAlgorithm, listedObject.ChecksumAlgorithms),
                () => Assert.Equal(expectedType, listedObject.ChecksumType),
                () => Assert.Contains(expectedAlgorithm, listedVersion.ChecksumAlgorithms),
                () => Assert.Equal(expectedType, listedVersion.ChecksumType));
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            if (created)
                Assert.Equal(204, (await client.DeleteObjectAsync(bucket, key, request => request.VersionId = version, cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, key, token: cleanup.Token)).StatusCode);
        }
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi, ChecksumAlgorithm.Md5, ChecksumAlgorithm.Sha512, ChecksumAlgorithm.XxHash64, ChecksumAlgorithm.XxHash3, ChecksumAlgorithm.XxHash128)]
    public async Task ExtendedChecksumMetadataInObjectListings(S3Provider provider, string bucket, ISimpleClient client, ChecksumAlgorithm algorithm)
    {
        byte[] data = "listing-extended-checksum-metadata"u8.ToArray();
        byte[] checksum = ComputeChecksum(algorithm, data);
        string key = "listing-extended-checksum-metadata-" + algorithm.ToString().ToLowerInvariant() + "-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        string? version = null;
        bool created = false;

        try
        {
            PutObjectResponse put = await client.PutObjectDataAsync(bucket, key, data, request =>
            {
                request.ChecksumAlgorithm = algorithm;
                request.Checksum = checksum;
            }, token);

            if (provider is S3Provider.BackBlazeB2 or S3Provider.Wasabi)
            {
                Assert.False(put.IsSuccess);
                Assert.Equal(provider == S3Provider.BackBlazeB2 ? 501 : 400, put.StatusCode);
                return;
            }

            Assert.True(put.IsSuccess, $"PUT: {put.StatusCode} {put.Error?.Code}");
            created = true;
            version = put.VersionId;
            await AssertReadAsync(client, bucket, key, version, data, algorithm, checksum, token);

            ListObjectsResponse objects = await client.ListObjectsAsync(bucket, request => request.Prefix = key, token);
            ListObjectVersionsResponse versions = await client.ListObjectVersionsAsync(bucket, request => request.Prefix = key, token);
            Assert.True(objects.IsSuccess, $"ListObjects: {objects.StatusCode} {objects.Error?.Code}");
            Assert.True(versions.IsSuccess, $"ListObjectVersions: {versions.StatusCode} {versions.Error?.Code}");
            S3Object listedObject = Assert.Single(objects.Objects, item => item.ObjectKey == key);
            S3Version listedVersion = Assert.Single(versions.Versions, item => item.ObjectKey == key && (version == null || item.VersionId == version));
            ChecksumAlgorithm expectedAlgorithm = algorithm;
            ChecksumType expectedType = provider == S3Provider.AmazonS3 ? ChecksumType.FullObject : ChecksumType.Unknown;
            Assert.Multiple(() => Assert.Contains(expectedAlgorithm, listedObject.ChecksumAlgorithms),
                () => Assert.Equal(expectedType, listedObject.ChecksumType),
                () => Assert.Contains(expectedAlgorithm, listedVersion.ChecksumAlgorithms),
                () => Assert.Equal(expectedType, listedVersion.ChecksumType));
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            if (created)
                Assert.Equal(204, (await client.DeleteObjectAsync(bucket, key, request => request.VersionId = version, cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, key, token: cleanup.Token)).StatusCode);
        }
    }

    private static async Task AssertReadAsync(ISimpleClient client, string bucket, string key, string? version, byte[] expectedData,
                                              ChecksumAlgorithm uploadAlgorithm, byte[] uploadChecksum, CancellationToken token, string? expectedAuditMetadata = null)
    {
        HeadObjectResponse head = await client.HeadObjectAsync(bucket, key, request =>
        {
            request.VersionId = version;
            request.EnableChecksum = true;
        }, token);
        using GetObjectResponse get = await client.GetObjectAsync(bucket, key, request =>
        {
            request.VersionId = version;
            request.EnableChecksum = true;
        }, token);
        Assert.True(head.IsSuccess, $"HEAD version {version}: {head.StatusCode} {head.Error?.Code}");
        Assert.True(get.IsSuccess, $"GET version {version}: {get.StatusCode} {get.Error?.Code}");
        Assert.Equal(expectedData, await get.Content.AsDataAsync(token: token));
        if (expectedAuditMetadata != null)
            Assert.Equal(expectedAuditMetadata, get.Metadata["audit"]);
        ChecksumAlgorithm expectedAlgorithm = uploadAlgorithm;
        byte[] expectedChecksum = uploadChecksum;
        ChecksumType expectedType = ChecksumType.FullObject;
        Assert.Multiple(() => Assert.Equal(expectedAlgorithm, head.ChecksumAlgorithm),
            () => Assert.Equal(expectedChecksum, head.Checksum),
            () => Assert.Equal(expectedType, head.ChecksumType),
            () => Assert.Equal(expectedAlgorithm, get.ChecksumAlgorithm),
            () => Assert.Equal(expectedChecksum, get.Checksum),
            () => Assert.Equal(expectedType, get.ChecksumType));
    }

    private static byte[] ComputeChecksum(ChecksumAlgorithm algorithm, byte[] data)
    {
        using HashAlgorithm hash = ChecksumHashAlgorithm.Create(algorithm);
        return hash.ComputeHash(data);
    }
}