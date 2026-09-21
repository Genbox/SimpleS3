using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Code;

[SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "This established provider-test fixture name is intentionally generic.")]
public abstract class TestBase
{
    private static readonly ConcurrentDictionary<S3Provider, SemaphoreSlim> _temporaryBucketLocks = new ConcurrentDictionary<S3Provider, SemaphoreSlim>();

    protected static string GetTemporaryBucket() => UtilityHelper.GetTemporaryBucket();

    protected static void AssertOwnerGrant(S3Identity? owner, S3Grant grant)
    {
        Assert.NotNull(owner);
        Assert.False(string.IsNullOrWhiteSpace(owner.Id));
        Assert.Equal(S3Permission.FullControl, grant.Permission);

        switch (grant.Grantee.Type)
        {
            case GrantType.CanonicalUser:
                Assert.Equal(owner.Id, grant.Grantee.Id);
                break;
            case GrantType.AmazonCustomerByEmail:
                Assert.False(string.IsNullOrWhiteSpace(owner.Name));
                Assert.Equal(owner.Name, grant.Grantee.EmailAddress, StringComparer.OrdinalIgnoreCase);
                break;
            default:
                Assert.Fail($"Expected an owner grant identified by canonical ID or email, but got {grant.Grantee.Type}.");
                break;
        }
    }

    protected static async Task<UploadPartResponse> UploadTestPartAsync(ISimpleClient client, string bucket, string key, string uploadId, int partNumber, byte[] data, Action<UploadPartRequest>? configure = null)
    {
        await using MemoryStream content = new MemoryStream(data);
        UploadPartResponse response = await client.UploadPartAsync(bucket, key, partNumber, uploadId, content, configure, TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        return response;
    }

    protected static async Task AssertObjectContentAsync(ISimpleClient client, string bucket, string key, byte[] expected, Action<GetObjectRequest>? configure = null)
    {
        using GetObjectResponse response = await client.GetObjectAsync(bucket, key, configure, TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(expected, await response.Content.AsDataAsync(TestContext.Current.CancellationToken));
    }

    protected static void ConfigureResponseHeaderOverride(HeadObjectRequest request, string header, DateTimeOffset expires)
    {
        switch (header)
        {
            case "cache-control": request.ResponseCacheControl.Add(CacheControlType.MaxAge, 42); break;
            case "disposition": request.ResponseContentDisposition.Set(ContentDispositionType.Attachment, "filename.txt"); break;
            case "encoding": request.ResponseContentEncoding.Add(ContentEncodingType.Gzip); break;
            case "language": request.ResponseContentLanguage.Add("da-DK"); break;
            case "type": request.ResponseContentType.Set("text/html", "utf-8"); break;
            case "expires": request.ResponseExpires = expires; break;
            default: throw new ArgumentOutOfRangeException(nameof(header));
        }
    }

    protected static void AssertResponseHeaderOverride(HeadObjectResponse response, string header, DateTimeOffset expires)
    {
        Assert.Equal(200, response.StatusCode);

        switch (header)
        {
            case "cache-control": Assert.Equal("max-age=42", response.CacheControl); break;
            case "disposition": Assert.Equal("attachment; filename=\"filename.txt\"", response.ContentDisposition); break;
            case "encoding": Assert.Equal("gzip", response.ContentEncoding); break;
            case "language": Assert.Equal("da-DK", response.ContentLanguage); break;
            case "type": Assert.Equal("text/html; charset=utf-8", response.ContentType); break;
            case "expires": Assert.Equal(expires.UtcDateTime, response.ExpiresOn!.Value.UtcDateTime, TimeSpan.FromSeconds(1)); break;
            default: throw new ArgumentOutOfRangeException(nameof(header));
        }
    }

    protected static async Task AllowSseCustomerKeysAsync(S3Provider provider, ISimpleClient client, string bucketName)
    {
        if (provider != S3Provider.AmazonS3)
            return;

        S3ServerSideEncryptionRule rule = new S3ServerSideEncryptionRule(SseAlgorithm.Aes256);
        rule.BlockedEncryptionTypes.Add(BlockedEncryptionType.None);

        PutBucketEncryptionResponse resp = await client.PutBucketEncryptionAsync(bucketName, [rule], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, resp.StatusCode);
    }

    protected static async Task<T> ReadExpectedAsync<T>(Func<CancellationToken, Task<T>> read, Func<T, bool> isExpected, string expectation, CancellationToken token, int attempts = 20, TimeSpan? interval = null)
    {
        T response = await read(token);

        for (int poll = 1; !isExpected(response) && poll < attempts; poll++)
        {
            await Task.Delay(interval ?? TimeSpan.FromMilliseconds(500), token);
            response = await read(token);
        }

        Assert.True(isExpected(response), $"Timed out waiting for {expectation}.");
        return response;
    }

    protected static Task<GetBucketLifecycleConfigurationResponse> ReadExpectedLifecycleConfigurationAsync(ISimpleClient client, string bucketName, Func<GetBucketLifecycleConfigurationResponse, bool> isExpected, CancellationToken token)
        => ReadExpectedAsync(t => client.GetBucketLifecycleConfigurationAsync(bucketName, token: t), isExpected, "the expected lifecycle configuration", token);

    protected static async Task CreateTempBucketAsync(S3Provider provider, ISimpleClient client, Func<string, Task> action, Action<CreateBucketRequest>? config = null)
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        SemaphoreSlim bucketLock = _temporaryBucketLocks.GetOrAdd(provider, static _ => new SemaphoreSlim(1, 1));
        await bucketLock.WaitAsync(token);

        string tempBucket = GetTemporaryBucket();

        try
        {
            CreateBucketResponse? createResponse = null;

            for (int attempt = 0; attempt < 5; attempt++)
            {
                createResponse = await client.CreateBucketAsync(tempBucket, config, token);

                if (createResponse.StatusCode != 429)
                    break;

                await Task.Delay(TimeSpan.FromSeconds(1 << attempt), token);
            }

            Assert.NotNull(createResponse);
            Assert.Equal(200, createResponse.StatusCode);

            Exception? testFailure = null;

            try
            {
                await action(tempBucket);
            }
            catch (Exception ex)
            {
                testFailure = ex;
                throw;
            }
            finally
            {
                using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                (Exception? cleanupFailure, bool deferred) = await UtilityHelper.TryDeleteTemporaryBucketAsync(provider, client, tempBucket, token: cleanup.Token);

                if (cleanupFailure != null)
                {
                    string cleanupStatus = deferred ? "deferred" : "failed";
                    TestContext.Current.TestOutputHelper?.WriteLine($"Temporary bucket cleanup {cleanupStatus} for '{tempBucket}' on {provider}: {cleanupFailure}");

                    if (testFailure == null && !deferred)
                        ExceptionDispatchInfo.Capture(cleanupFailure).Throw();
                }
            }
        }
        finally
        {
            bucketLock.Release();
        }
    }
}