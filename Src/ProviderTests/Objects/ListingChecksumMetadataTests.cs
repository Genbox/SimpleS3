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
        => await AssertChecksumOverwritePreservesVersionMetadataAsync(provider, bucket, client, ChecksumAlgorithm.Crc32, ChecksumAlgorithm.Crc64Nvme);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task Sha1OverwritePreservesVersionMetadata(S3Provider provider, string bucket, ISimpleClient client) => await AssertChecksumOverwritePreservesVersionMetadataAsync(provider,
        bucket,
        client,
        ChecksumAlgorithm.Sha1,
        ChecksumAlgorithm.Sha256);

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi, true, false)]
    public async Task OrdinaryAndMultipartOverwritesPreserveBothChecksumVersions(S3Provider provider, string bucket, ISimpleClient client, bool multipartFirst)
        => await AssertChecksumTransitionAsync(provider, bucket, client, multipartFirst);

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

            await AssertReadAsync(client, bucket, key, null,
                multipartFirst ? ordinaryData : multipartData,
                multipartFirst ? ordinaryAlgorithm : multipartAlgorithm,
                multipartFirst ? ordinaryChecksum : multipartChecksum,
                token, multipartFirst ? "ordinary" : "multipart");
            await AssertReadAsync(client, bucket, key, multipartVersion, multipartData, multipartAlgorithm, multipartChecksum, token, "multipart");
            await AssertReadAsync(client, bucket, key, ordinaryVersion, ordinaryData, ordinaryAlgorithm, ordinaryChecksum, token, "ordinary");

            (ListObjectsResponse objects, ListObjectVersionsResponse listedVersions) = await ReadChecksumListingsAsync(client, bucket, key, secondVersion, 2,
                multipartFirst ? ordinaryAlgorithm : multipartAlgorithm, token);
            S3Object current = Assert.Single(objects.Objects, item => item.ObjectKey == key);
            S3Version multipartListed = Assert.Single(listedVersions.Versions, item => item.ObjectKey == key && item.VersionId == multipartVersion);
            S3Version ordinaryListed = Assert.Single(listedVersions.Versions, item => item.ObjectKey == key && item.VersionId == ordinaryVersion);
            ChecksumType listedType = provider is S3Provider.AmazonS3 or S3Provider.BackBlazeB2 ? ChecksumType.FullObject : ChecksumType.Unknown;
            Assert.Multiple(() => Assert.Contains(multipartFirst ? ordinaryAlgorithm : multipartAlgorithm, current.ChecksumAlgorithms),
                () => Assert.Equal(listedType, current.ChecksumType),
                () => Assert.Contains(multipartAlgorithm, multipartListed.ChecksumAlgorithms),
                () => Assert.Equal(listedType, multipartListed.ChecksumType),
                () => Assert.Equal(!multipartFirst, multipartListed.IsLatest),
                () => Assert.Contains(ordinaryAlgorithm, ordinaryListed.ChecksumAlgorithms),
                () => Assert.Equal(listedType, ordinaryListed.ChecksumType),
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

            await AssertReadAsync(client, bucket, key, null, secondData, secondAlgorithm, secondChecksum, token);
            await AssertReadAsync(client, bucket, key, first.VersionId, firstData, firstAlgorithm, firstChecksum, token);
            await AssertReadAsync(client, bucket, key, second.VersionId, secondData, secondAlgorithm, secondChecksum, token);

            (ListObjectsResponse objects, ListObjectVersionsResponse listedVersions) = await ReadChecksumListingsAsync(client, bucket, key, second.VersionId, 2, secondAlgorithm, token);
            S3Object current = Assert.Single(objects.Objects, item => item.ObjectKey == key);
            S3Version firstListed = Assert.Single(listedVersions.Versions, item => item.ObjectKey == key && item.VersionId == first.VersionId);
            S3Version secondListed = Assert.Single(listedVersions.Versions, item => item.ObjectKey == key && item.VersionId == second.VersionId);
            ChecksumType listedType = provider is S3Provider.AmazonS3 or S3Provider.BackBlazeB2 ? ChecksumType.FullObject : ChecksumType.Unknown;
            Assert.Multiple(() => Assert.Contains(secondAlgorithm, current.ChecksumAlgorithms),
                () => Assert.Equal(listedType, current.ChecksumType),
                () => Assert.Contains(firstAlgorithm, firstListed.ChecksumAlgorithms),
                () => Assert.Equal(listedType, firstListed.ChecksumType),
                () => Assert.False(firstListed.IsLatest),
                () => Assert.Contains(secondAlgorithm, secondListed.ChecksumAlgorithms),
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

            (ListObjectsResponse objects, ListObjectVersionsResponse versions) = await ReadChecksumListingsAsync(client, bucket, key, version, 1, algorithm, token);
            S3Object listedObject = Assert.Single(objects.Objects, item => item.ObjectKey == key);
            S3Version listedVersion = Assert.Single(versions.Versions, item => item.ObjectKey == key && (version == null || item.VersionId == version));
            ChecksumType expectedType = provider is S3Provider.AmazonS3 or S3Provider.BackBlazeB2 ? ChecksumType.FullObject : ChecksumType.Unknown;
            Assert.Multiple(() => Assert.Contains(algorithm, listedObject.ChecksumAlgorithms),
                () => Assert.Equal(expectedType, listedObject.ChecksumType),
                () => Assert.Contains(algorithm, listedVersion.ChecksumAlgorithms),
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
    [MultipleProvidersWithData(S3Provider.AmazonS3, ChecksumAlgorithm.Md5, ChecksumAlgorithm.Sha512, ChecksumAlgorithm.XxHash64, ChecksumAlgorithm.XxHash3, ChecksumAlgorithm.XxHash128)]
    public async Task ExtendedChecksumMetadataInObjectListings(S3Provider _, string bucket, ISimpleClient client, ChecksumAlgorithm algorithm)
    {
        byte[] data = "listing-extended-checksum-metadata"u8.ToArray();
        byte[] checksum = ComputeChecksum(algorithm, data);
        string key = "listing-extended-checksum-metadata-" + algorithm.ToString().ToLowerInvariant() + "-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectDataAsync(bucket, key, data, request =>
        {
            request.ChecksumAlgorithm = algorithm;
            request.Checksum = checksum;
        }, token), async put =>
        {
            string? version = put.VersionId;
            await AssertReadAsync(client, bucket, key, version, data, algorithm, checksum, token);

            (ListObjectsResponse objects, ListObjectVersionsResponse versions) = await ReadChecksumListingsAsync(client, bucket, key, version, 1, algorithm, token);
            S3Object listedObject = Assert.Single(objects.Objects, item => item.ObjectKey == key);
            S3Version listedVersion = Assert.Single(versions.Versions, item => item.ObjectKey == key && (version == null || item.VersionId == version));
            Assert.Multiple(() => Assert.Contains(algorithm, listedObject.ChecksumAlgorithms),
                () => Assert.Equal(ChecksumType.FullObject, listedObject.ChecksumType),
                () => Assert.Contains(algorithm, listedVersion.ChecksumAlgorithms),
                () => Assert.Equal(ChecksumType.FullObject, listedVersion.ChecksumType));
        });
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.BackBlazeB2 | S3Provider.Wasabi, ChecksumAlgorithm.Md5, ChecksumAlgorithm.Sha512, ChecksumAlgorithm.XxHash64, ChecksumAlgorithm.XxHash3, ChecksumAlgorithm.XxHash128)]
    public async Task UnsupportedExtendedChecksumRejected(S3Provider provider, string _, ISimpleClient client, ChecksumAlgorithm algorithm) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        byte[] data = "unsupported-checksum"u8.ToArray();
        PutObjectResponse response = await client.PutObjectDataAsync(bucket, "checksum", data, request =>
        {
            request.ChecksumAlgorithm = algorithm;
            request.Checksum = ComputeChecksum(algorithm, data);
        }, TestContext.Current.CancellationToken);
        Assert.Equal(provider == S3Provider.BackBlazeB2 ? 501 : 400, response.StatusCode);
    });

    private static Task<(ListObjectsResponse Objects, ListObjectVersionsResponse Versions)> ReadChecksumListingsAsync(ISimpleClient client, string bucket, string key,
                                                                                                                      string? latestVersion, int versionCount, ChecksumAlgorithm algorithm, CancellationToken token)
        => ReadExpectedAsync(async t =>
            {
                ListObjectsResponse objects = await client.ListObjectsAsync(bucket, request => request.Prefix = key, t);
                ListObjectVersionsResponse versions = await client.ListObjectVersionsAsync(bucket, request => request.Prefix = key, t);
                Assert.True(objects.IsSuccess, objects.Error?.Message);
                Assert.True(versions.IsSuccess, versions.Error?.Message);
                return (Objects: objects, Versions: versions);
            }, result => result.Objects.Objects.Any(item => item.ObjectKey == key && item.ChecksumAlgorithms.Contains(algorithm)) &&
                         result.Versions.Versions.Count == versionCount && result.Versions.Versions.All(item => item.ObjectKey == key) &&
                         result.Versions.Versions.Any(item => item.ObjectKey == key && item.IsLatest && (latestVersion == null || item.VersionId == latestVersion) && item.ChecksumAlgorithms.Contains(algorithm)),
            $"checksum listings for '{key}'", token);

    private static async Task AssertReadAsync(ISimpleClient client, string bucket, string key, string? version, byte[] expectedData,
                                              ChecksumAlgorithm uploadAlgorithm, byte[] uploadChecksum, CancellationToken token, string? expectedAuditMetadata = null)
    {
        HeadObjectResponse head = await ReadExpectedAsync(t => client.HeadObjectAsync(bucket, key, request =>
            {
                request.VersionId = version;
                request.EnableChecksum = true;
            }, t), response => response.IsSuccess && response.ChecksumAlgorithm == uploadAlgorithm &&
                               response.ContentLength == expectedData.Length && response.Checksum != null && response.Checksum.SequenceEqual(uploadChecksum),
            $"checksum metadata for '{key}' version '{version ?? "latest"}'", token);
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
        Assert.Multiple(() => Assert.Equal(uploadAlgorithm, head.ChecksumAlgorithm),
            () => Assert.Equal(uploadChecksum, head.Checksum),
            () => Assert.Equal(ChecksumType.FullObject, head.ChecksumType),
            () => Assert.Equal(uploadAlgorithm, get.ChecksumAlgorithm),
            () => Assert.Equal(uploadChecksum, get.Checksum),
            () => Assert.Equal(ChecksumType.FullObject, get.ChecksumType));
    }

    private static byte[] ComputeChecksum(ChecksumAlgorithm algorithm, byte[] data)
    {
        using HashAlgorithm hash = ChecksumHashAlgorithm.Create(algorithm);
        return hash.ComputeHash(data);
    }
}