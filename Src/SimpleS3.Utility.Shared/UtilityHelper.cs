using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.AmazonS3.Extensions;
using Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.Wasabi.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HttpVersion = Genbox.SimpleS3.Core.Common.HttpVersion;

namespace Genbox.SimpleS3.Utility.Shared;

public static class UtilityHelper
{
    public static string GetProfileName(S3Provider provider) => "TestSetup-" + provider;

    public static string GetTestBucket(IProfile profile) => "testbucket-" + profile.KeyId[..8].ToLowerInvariant();

    public static string GetTemporaryBucket() => "tempbucket-" + Guid.NewGuid();

    public static bool IsTestBucket(string bucketName, IProfile profile) => string.Equals(bucketName, GetTestBucket(profile), StringComparison.Ordinal);

    public static bool IsTemporaryBucket(string bucketName)
    {
        const string prefix = "tempbucket-";
        return bucketName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && Guid.TryParseExact(bucketName[prefix.Length..], "D", out _);
    }

    public static ServiceProvider CreateSimpleS3(S3Provider provider, string profileName, bool enableRetry, Action<SimpleS3Config>? configure = null, Action<IServiceCollection>? configureServices = null)
    {
        ServiceCollection services = new ServiceCollection();
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services, configure);

        IConfigurationRoot configRoot = new ConfigurationBuilder()
                                        .SetBasePath(Environment.CurrentDirectory)
                                        .AddJsonFile("Config.json", false)
                                        .Build();
        using IDisposable configRootLifetime = (IDisposable)configRoot;

        IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

        services.Configure<HttpClientFactoryConfig>(x => x.HttpVersion = HttpVersion.Http2);

        if (enableRetry)
            httpBuilder.UseRetryAndTimeout();

        IConfigurationSection proxySection = configRoot.GetSection("Proxy");

        if (proxySection.GetValue<bool>("UseProxy"))
            httpBuilder.UseProxy(new WebProxy(proxySection.GetValue<string>("ProxyAddress")));

        coreBuilder.UseProfileManager()
                   .BindConfigToProfile(profileName)
                   .UseConsoleSetup();

        if (provider == S3Provider.AmazonS3)
            coreBuilder.UseAmazonS3();
        else if (provider == S3Provider.BackBlazeB2)
            coreBuilder.UseBackBlazeB2();
        else if (provider == S3Provider.GoogleCloudStorage)
            coreBuilder.UseGoogleCloudStorage();
        else if (provider == S3Provider.Wasabi)
            coreBuilder.UseWasabi();
        else
            throw new ArgumentOutOfRangeException(nameof(provider), provider, null);

        configureServices?.Invoke(services);
        return services.BuildServiceProvider();
    }

    public static IProfile GetOrSetupProfile(IServiceProvider serviceProvider, S3Provider provider, string profileName)
    {
        //Check if there is a profile for this provider
        IProfileManager profileManager = serviceProvider.GetRequiredService<IProfileManager>();
        IProfile? profile = profileManager.GetProfile(profileName);

        if (profile == null)
        {
            Console.WriteLine("The profile " + profileName + " does not exist.");

            if (provider == S3Provider.AmazonS3)
                Console.WriteLine("You can create a new API key at https://console.aws.amazon.com/iam/home?#/security_credentials");
            else if (provider == S3Provider.BackBlazeB2)
                Console.WriteLine("You can create a new API key at https://secure.backblaze.com/app_keys.htm");

            IProfileSetup profileSetup = serviceProvider.GetRequiredService<IProfileSetup>();
            return profileSetup.SetupProfile(profileName);
        }

        return profile;
    }

    private static async Task<int> EmptyBucketAsync(S3Provider provider, ISimpleClient client, string bucket, bool useUrlEncodedListings, List<string> failures, CancellationToken token)
    {
        BucketDeletionOptions options = new BucketDeletionOptions(false, true);

        if (provider == S3Provider.AmazonS3)
        {
            GetBucketVersioningResponse versioning = await client.GetBucketVersioningAsync(bucket, token: token);
            if (!versioning.IsSuccess)
                throw new InvalidOperationException(versioning.Error?.Message ?? "Unable to read bucket versioning before cleanup.");
            GetObjectLockConfigurationResponse objectLock = await client.GetObjectLockConfigurationAsync(bucket, token: token);
            if (!objectLock.IsSuccess && objectLock.StatusCode != 404)
                throw new InvalidOperationException(objectLock.Error?.Message ?? "Unable to read bucket Object Lock before cleanup.");
            options = new BucketDeletionOptions(!versioning.Status, objectLock.IsSuccess && objectLock.Enabled);
        }

        Dictionary<(string? ObjectKey, string? VersionId), S3DeleteError> errors = new Dictionary<(string? ObjectKey, string? VersionId), S3DeleteError>();
        int operationErrors = await AbortAllMultipartUploadsAsync(client, bucket, useUrlEncodedListings, failures, token);

        if (provider != S3Provider.GoogleCloudStorage)
        {
            await foreach (S3DeleteError error in DeleteAllObjectVersionsAsync(provider, client, bucket, useUrlEncodedListings, options, token).WithCancellation(token))
                errors[(error.ObjectKey, error.VersionId)] = error;
        }

        // Version listings can omit a current null version. Delete every visible current object,
        // then scan versions again to remove any delete markers created by this pass.
        await foreach (S3Object obj in client.ListAllObjectsAsync(bucket, request => request.EncodingType = GetListingEncoding(useUrlEncodedListings), token).WithCancellation(token))
        {
            DeleteObjectResponse deleteResponse = provider == S3Provider.GoogleCloudStorage
                ? await client.DeleteObjectAsync(bucket, obj.ObjectKey, token: token)
                : await DeleteObjectVersionAsync(client, bucket, obj.ObjectKey, null, options, token);

            if (!deleteResponse.IsSuccess && deleteResponse.StatusCode != 404)
                errors[(obj.ObjectKey, null)] = new S3DeleteError(obj.ObjectKey, deleteResponse.Error?.Code ?? ErrorCode.Unknown, deleteResponse.Error?.Message ?? "Unable to delete object.", null);
            else
                errors.Remove((obj.ObjectKey, null));
        }

        if (provider != S3Provider.GoogleCloudStorage)
        {
            await foreach (S3DeleteError error in DeleteAllObjectVersionsAsync(provider, client, bucket, useUrlEncodedListings, options, token).WithCancellation(token))
                errors[(error.ObjectKey, error.VersionId)] = error;
        }

        // Retry failed versions individually so governance retention and legal holds can be handled.
        if (errors.Count > 0 && provider != S3Provider.GoogleCloudStorage)
        {
            foreach (S3DeleteError error in errors.Values.ToArray())
            {
                if (string.IsNullOrEmpty(error.ObjectKey))
                    continue;

                S3DeleteError? remaining;

                try
                {
                    remaining = await TryForceDeleteObjectVersionAsync(client, bucket, error, options, token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    remaining = new S3DeleteError(error.ObjectKey, error.Code, ex.Message, error.VersionId);
                }

                if (remaining == null)
                    errors.Remove((error.ObjectKey, error.VersionId));
                else
                    errors[(error.ObjectKey, error.VersionId)] = remaining;
            }
        }

        if (provider != S3Provider.GoogleCloudStorage)
        {
            List<S3DeleteInfo> residue = await ListRemainingObjectVersionsAsync(client, bucket, useUrlEncodedListings, token);

            foreach (S3DeleteInfo info in residue)
            {
                S3DeleteError residualError = new S3DeleteError(info.ObjectKey, ErrorCode.Unknown, "Object version remained after batch deletion.", info.VersionId);
                S3DeleteError? remaining = await TryForceDeleteObjectVersionAsync(client, bucket, residualError, options, token);

                if (remaining == null)
                    errors.Remove((info.ObjectKey, info.VersionId));
                else
                    errors[(info.ObjectKey, info.VersionId)] = remaining;
            }

            List<S3DeleteInfo> finalResidue = await ListRemainingObjectVersionsAsync(client, bucket, useUrlEncodedListings, token);
            HashSet<(string ObjectKey, string? VersionId)> remainingVersions = finalResidue.Select(info => (info.ObjectKey, info.VersionId)).ToHashSet();

            foreach ((string? objectKey, string? versionId) in errors.Keys.Where(key => !string.IsNullOrEmpty(key.ObjectKey)).ToArray())
            {
                if (!remainingVersions.Contains((objectKey!, versionId)))
                    errors.Remove((objectKey, versionId));
            }

            foreach (S3DeleteInfo info in finalResidue)
                errors[(info.ObjectKey, info.VersionId)] = await DescribeRemainingObjectVersionAsync(client, bucket, info, token);
        }

        foreach (S3DeleteError error in errors.Values)
            failures.Add(FormatDeleteError(error));

        return operationErrors + errors.Count;
    }

    private static async Task<int> AbortAllMultipartUploadsAsync(ISimpleClient client, string bucket, bool useUrlEncodedListings, List<string> failures, CancellationToken token)
    {
        HashSet<(string ObjectKey, string UploadId)> attempted = [];
        int errors = 0;

        while (true)
        {
            int newUploads = 0;

            // Restart from the first page after every pass. Some providers can skip same-key uploads
            // when an upload used as the continuation marker is aborted during enumeration.
            await foreach (S3Upload upload in client.ListAllMultipartUploadsAsync(bucket, request => request.EncodingType = GetListingEncoding(useUrlEncodedListings), token).WithCancellation(token))
            {
                if (!attempted.Add((upload.ObjectKey, upload.UploadId)))
                    continue;

                newUploads++;
                AbortMultipartUploadResponse response = await client.AbortMultipartUploadAsync(bucket, upload.ObjectKey, upload.UploadId, token: token);

                if (!response.IsSuccess)
                {
                    errors++;
                    failures.Add($"Multipart upload '{upload.ObjectKey}' ({upload.UploadId}): HTTP {response.StatusCode}: {response.Error?.Code}: {response.Error?.Message}");
                }
            }

            if (newUploads == 0 || errors > 0)
                return errors;
        }
    }

    private static async Task<S3DeleteError?> TryForceDeleteObjectVersionAsync(ISimpleClient client, string bucket, S3DeleteError error, BucketDeletionOptions options, CancellationToken token)
    {
        DeleteObjectResponse deleteResponse = await DeleteObjectVersionAsync(client, bucket, error.ObjectKey!, error.VersionId, options, token);

        if (deleteResponse.IsSuccess || deleteResponse.StatusCode == 404)
            return null;

        PutObjectLegalHoldResponse legalHoldResponse = await client.PutObjectLegalHoldAsync(bucket, error.ObjectKey!, false, request => request.VersionId = error.VersionId, token);

        if (legalHoldResponse.IsSuccess)
        {
            deleteResponse = await DeleteObjectVersionAsync(client, bucket, error.ObjectKey!, error.VersionId, options, token);

            if (deleteResponse.IsSuccess || deleteResponse.StatusCode == 404)
                return null;
        }

        GetObjectRetentionResponse retentionResponse = await client.GetObjectRetentionAsync(bucket, error.ObjectKey!, request => request.VersionId = error.VersionId, token);
        bool activeComplianceRetention = retentionResponse.IsSuccess &&
                                         retentionResponse.LockMode == LockMode.Compliance &&
                                         retentionResponse.LockRetainUntil > DateTimeOffset.UtcNow;
        string retention = activeComplianceRetention ? $" Active compliance retention until {retentionResponse.LockRetainUntil:O}." : string.Empty;
        string message = deleteResponse.Error?.Message ?? error.Message ?? "Unable to delete object version.";
        return new S3DeleteError(error.ObjectKey, deleteResponse.Error?.Code ?? error.Code, message + retention, error.VersionId);
    }

    private static Task<DeleteObjectResponse> DeleteObjectVersionAsync(ISimpleClient client, string bucket, string objectKey, string? versionId, BucketDeletionOptions options, CancellationToken token) =>
        client.DeleteObjectAsync(bucket, objectKey, request =>
        {
            bool unversionedObject = options.OmitNullVersionId && (versionId == null || versionId == "null");
            request.VersionId = unversionedObject ? null : versionId;
            request.BypassGovernanceRetention = !unversionedObject && options.BypassGovernanceRetention ? true : null;
        }, token);

    private static async Task<List<S3DeleteInfo>> ListRemainingObjectVersionsAsync(ISimpleClient client, string bucket, bool useUrlEncodedListings, CancellationToken token)
    {
        HashSet<(string Key, string? Version)> seenMarkers = [];
        List<S3DeleteInfo> versions = [];
        string? keyMarker = null;
        string? versionIdMarker = null;

        while (true)
        {
            ListObjectVersionsResponse response = await client.ListObjectVersionsAsync(bucket, request =>
            {
                request.EncodingType = GetListingEncoding(useUrlEncodedListings);
                request.KeyMarker = keyMarker;
                request.VersionIdMarker = versionIdMarker;
            }, token);

            if (!response.IsSuccess)
                throw new InvalidOperationException(response.Error?.Message ?? "Unable to verify remaining object versions.");

            versions.AddRange(response.Versions.Select(version => new S3DeleteInfo(version.ObjectKey, version.VersionId)));
            versions.AddRange(response.DeleteMarkers.Select(marker => new S3DeleteInfo(marker.ObjectKey, marker.VersionId)));

            if (!response.IsTruncated)
                return versions;

            keyMarker = response.NextKeyMarker;
            versionIdMarker = response.NextVersionIdMarker;

            if (string.IsNullOrEmpty(keyMarker) || !seenMarkers.Add((keyMarker, versionIdMarker)))
                throw new InvalidOperationException("Object version verification returned missing or non-progressing markers.");
        }
    }

    private static async Task<S3DeleteError> DescribeRemainingObjectVersionAsync(ISimpleClient client, string bucket, S3DeleteInfo info, CancellationToken token)
    {
        GetObjectRetentionResponse response = await client.GetObjectRetentionAsync(bucket, info.ObjectKey, request => request.VersionId = info.VersionId, token);
        bool activeComplianceRetention = response.IsSuccess && response.LockMode == LockMode.Compliance && response.LockRetainUntil > DateTimeOffset.UtcNow;
        string message = activeComplianceRetention
            ? $"Object version remained after deletion. Active compliance retention until {response.LockRetainUntil:O}."
            : "Object version remained after deletion even though the provider reported success.";
        return new S3DeleteError(info.ObjectKey, ErrorCode.Unknown, message, info.VersionId);
    }

    private static string FormatDeleteError(S3DeleteError error) =>
        $"Object '{error.ObjectKey ?? "<unknown>"}', version '{error.VersionId ?? "<none>"}': {error.Code}: {error.Message}";

    public static async Task<(Exception? Failure, bool Deferred)> TryDeleteTemporaryBucketAsync(S3Provider provider, ISimpleClient client, string bucket, int maxAttempts = 6, bool useUrlEncodedListings = false, CancellationToken token = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxAttempts, 1);

        if (!IsTemporaryBucket(bucket))
            return (new ArgumentException("Only temporary buckets can be deleted by this operation.", nameof(bucket)), false);

        return await TryDeleteBucketAsync(provider, client, bucket, maxAttempts, useUrlEncodedListings, token);
    }

    [SuppressMessage("Style", "RCS1075", Justification = "Bucket deletion is still attempted after an emptying failure; its response determines whether this cleanup attempt succeeded.")]
    private static async Task<(Exception? Failure, bool Deferred)> TryDeleteBucketAsync(S3Provider provider, ISimpleClient client, string bucket, int maxAttempts, bool useUrlEncodedListings, CancellationToken token)
    {
        Exception? lastFailure = null;
        int lastEmptyErrors = -1;
        int? lastStatusCode = null;
        List<string> lastEmptyFailures = [];

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int errors = -1;
            List<string> emptyFailures = [];

            try
            {
                errors = await EmptyBucketAsync(provider, client, bucket, useUrlEncodedListings, emptyFailures, token);
                lastEmptyErrors = errors;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                emptyFailures.Add("Bucket emptying failed: " + ex.Message);
            }

            lastEmptyFailures = emptyFailures;

            try
            {
                DeleteBucketResponse response = await client.DeleteBucketAsync(bucket, token: token);
                lastStatusCode = response.StatusCode;

                if (response.IsSuccess || response.StatusCode == 404)
                    return (null, false);

                string message = response.Error?.Message ?? "Unknown error";
                string details = emptyFailures.Count == 0 ? string.Empty : " Failures: " + string.Join(" | ", emptyFailures);
                lastFailure = new InvalidOperationException($"Unable to delete bucket '{bucket}'. Status: {response.StatusCode}. Empty errors: {errors}. Message: {message}.{details}");
                if (provider == S3Provider.Wasabi && response.StatusCode == 424 && errors == 0)
                    return (lastFailure, true);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                lastFailure = ex;
            }

            if (attempt + 1 < maxAttempts)
                await Task.Delay(TimeSpan.FromSeconds(1 << Math.Min(attempt, 5)), token);
        }

        string finalDetails = lastEmptyFailures.Count == 0 ? string.Empty : " Failures: " + string.Join(" | ", lastEmptyFailures);
        lastFailure ??= new InvalidOperationException($"Unable to delete bucket '{bucket}'.{finalDetails}");
        bool deferred = provider == S3Provider.Wasabi && lastStatusCode == 424 && lastEmptyErrors == 0;
        return (lastFailure, deferred);
    }

    private static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersionsAsync(S3Provider provider, ISimpleClient client, string bucket, bool useUrlEncodedListings, BucketDeletionOptions options, [EnumeratorCancellation]CancellationToken token = default)
    {
        HashSet<(string Key, string? Version)> seenMarkers = [];
        Task<ListObjectVersionsResponse>? responseTask = null;
        CancellationTokenSource prefetchCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            responseTask = client.ListObjectVersionsAsync(bucket, request => request.EncodingType = GetListingEncoding(useUrlEncodedListings), prefetchCancellation.Token);

            ListObjectVersionsResponse response;

            do
            {
                response = await responseTask!;
                responseTask = null;

                if (!response.IsSuccess)
                {
                    yield return new S3DeleteError(string.Empty, response.Error?.Code ?? ErrorCode.Unknown, response.Error?.Message ?? "Unable to list object versions.", null);
                    yield break;
                }

                if (response.Versions.Count + response.DeleteMarkers.Count == 0 && !response.IsTruncated)
                    break;

                if (response.IsTruncated)
                {
                    string? keyMarker = response.NextKeyMarker;
                    string? versionIdMarker = response.NextVersionIdMarker;

                    if (string.IsNullOrEmpty(keyMarker) || !seenMarkers.Add((keyMarker, versionIdMarker)))
                    {
                        yield return new S3DeleteError(string.Empty, ErrorCode.Unknown, "Object version listing returned missing or non-progressing markers.", null);
                        yield break;
                    }

                    responseTask = client.ListObjectVersionsAsync(bucket, req =>
                    {
                        req.EncodingType = GetListingEncoding(useUrlEncodedListings);
                        req.KeyMarker = keyMarker;
                        req.VersionIdMarker = versionIdMarker;
                    }, prefetchCancellation.Token);
                }

                S3DeleteInfo[] delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)))
                                                .ToArray();

                //Google does not support DeleteObjects
                if (provider == S3Provider.GoogleCloudStorage)
                {
                    foreach (S3DeleteInfo info in delete)
                    {
                        DeleteObjectResponse deleteResponse = await client.DeleteObjectAsync(bucket, info.ObjectKey, info.VersionId, token: prefetchCancellation.Token);

                        if (!deleteResponse.IsSuccess)
                            yield return new S3DeleteError(info.ObjectKey, deleteResponse.Error?.Code ?? ErrorCode.Unknown, deleteResponse.Error?.Message ?? "Unable to delete object.", info.VersionId);
                    }
                }
                else
                {
                    S3DeleteInfo[] batchDelete = delete.Where(info => CanSerializeInXml(info.ObjectKey) && !(options.OmitNullVersionId && (info.VersionId == null || info.VersionId == "null"))).ToArray();

                    foreach (S3DeleteInfo info in delete.Where(info => !CanSerializeInXml(info.ObjectKey) || (options.OmitNullVersionId && (info.VersionId == null || info.VersionId == "null"))))
                    {
                        DeleteObjectResponse deleteResponse = await DeleteObjectVersionAsync(client, bucket, info.ObjectKey, info.VersionId, options, prefetchCancellation.Token);

                        if (!deleteResponse.IsSuccess && deleteResponse.StatusCode != 404)
                            yield return new S3DeleteError(info.ObjectKey, deleteResponse.Error?.Code ?? ErrorCode.Unknown, deleteResponse.Error?.Message ?? "Unable to delete object.", info.VersionId);
                    }

                    if (batchDelete.Length == 0)
                        continue;

                    DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucket, batchDelete, req =>
                    {
                        req.Quiet = false;
                        req.BypassGovernanceRetention = options.BypassGovernanceRetention ? true : null;
                    }, prefetchCancellation.Token);

                    if (!multiDelResponse.IsSuccess)
                    {
                        foreach (S3DeleteInfo info in batchDelete)
                            yield return new S3DeleteError(info.ObjectKey, multiDelResponse.Error?.Code ?? ErrorCode.Unknown, multiDelResponse.Error?.Message ?? "Unable to delete object.", info.VersionId);

                        yield break;
                    }

                    foreach (S3DeleteError error in multiDelResponse.Errors)
                        yield return error;
                }
            } while (response.IsTruncated);
        }
        finally
        {
            await prefetchCancellation.CancelAsync();
            await ObservePrefetchTaskAsync(responseTask);
            prefetchCancellation.Dispose();
        }
    }

    private static bool CanSerializeInXml(string value)
    {
        try
        {
            XmlConvert.VerifyXmlChars(value);
            return true;
        }
        catch (XmlException)
        {
            return false;
        }
    }

    private static EncodingType GetListingEncoding(bool useUrlEncodedListings) => useUrlEncodedListings ? EncodingType.Url : EncodingType.Unknown;

    [SuppressMessage("Usage", "VSTHRD003", Justification = "Cleanup intentionally observes the pre-started speculative prefetch task.")]
    private static async Task ObservePrefetchTaskAsync(Task? task)
    {
        if (task == null)
            return;

        try
        {
#pragma warning disable VSTHRD003 // This helper deliberately observes a prefetch task started by its caller.
            await task;
#pragma warning restore VSTHRD003
        }
        catch
        {
            // The abandoned prefetch is canceled and observed so it cannot surface later as an unobserved exception.
        }
    }

    private readonly record struct BucketDeletionOptions(bool OmitNullVersionId, bool BypassGovernanceRetention);
}