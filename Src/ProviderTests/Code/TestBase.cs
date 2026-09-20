using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Code;

[SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "This established provider-test fixture name is intentionally generic.")]
public abstract class TestBase
{
    private static readonly ConcurrentDictionary<S3Provider, SemaphoreSlim> _temporaryBucketLocks = new ConcurrentDictionary<S3Provider, SemaphoreSlim>();

    protected static string GetTemporaryBucket() => UtilityHelper.GetTemporaryBucket();

    protected static async Task AllowSseCustomerKeysAsync(S3Provider provider, ISimpleClient client, string bucketName)
    {
        if (provider != S3Provider.AmazonS3)
            return;

        S3ServerSideEncryptionRule rule = new S3ServerSideEncryptionRule(SseAlgorithm.Aes256);
        rule.BlockedEncryptionTypes.Add(BlockedEncryptionType.None);

        PutBucketEncryptionResponse resp = await client.PutBucketEncryptionAsync(bucketName, [rule], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, resp.StatusCode);
    }

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
                (Exception? cleanupFailure, bool cleanupDeferred) = await UtilityHelper.TryDeleteTemporaryBucketAsync(provider, client, tempBucket, token: token);

                if (cleanupFailure != null)
                {
                    string cleanupStatus = cleanupDeferred ? "deferred" : "failed";
                    TestContext.Current.TestOutputHelper?.WriteLine($"Temporary bucket cleanup {cleanupStatus} for '{tempBucket}': {cleanupFailure}");

                    if (testFailure == null && !cleanupDeferred)
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