using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Validation;
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
    public static IEnumerable<S3Provider> SelectProviders()
    {
        //Skip 'unknown'
        S3Provider[] choices = Enum.GetValues<S3Provider>().Skip(1).ToArray();

        bool success = false;
        int i;

        S3Provider[] returnChoices = new S3Provider[choices.Length];

        Console.WriteLine("Please select which providers you want to use. Use ',' to select more than one.");

        for (i = 0; i < choices.Length; i++)
            Console.WriteLine($"{i + 1}. {choices[i]}");

        do
        {
            string? input = Console.ReadLine();
            Validator.RequireNotNull(input);

            string[] userChoices = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            for (i = 0; i < userChoices.Length; i++)
            {
                string c = userChoices[i];

                if (!int.TryParse(c, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int intVal) || intVal <= 0 || intVal > choices.Length)
                {
                    Console.WriteLine($"Invalid input '{c}'. Try again.");
                    Array.Clear(returnChoices);
                    success = false;
                    break;
                }

                success = true;
                returnChoices[i] = choices[intVal - 1];
            }
        } while (!success);

        //Special case if 'All' was chosen
        if (returnChoices.Any(x => x == S3Provider.All))
            return choices.Take(choices.Length - 1);

        return returnChoices.Take(i);
    }

    public static string GetProfileName(S3Provider provider) => "TestSetup-" + provider;

    public static string GetTestBucket(IProfile profile) => "testbucket-" + profile.KeyId[..8].ToLowerInvariant();

    public static string GetTemporaryBucket() => "tempbucket-" + Guid.NewGuid();

    public static bool IsTestBucket(string bucketName, IProfile profile) => string.Equals(bucketName, GetTestBucket(profile), StringComparison.Ordinal);

    public static bool IsTemporaryBucket(string bucketName)
    {
        const string prefix = "tempbucket-";
        return bucketName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && Guid.TryParseExact(bucketName[prefix.Length..], "D", out _);
    }

    public static ServiceProvider CreateSimpleS3(S3Provider provider, string profileName, bool enableRetry, Action<SimpleS3Config>? configure = null)
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

    public static async Task<int> ForceEmptyBucketAsync(S3Provider provider, ISimpleClient client, string bucket, CancellationToken token = default)
    {
        Dictionary<(string? ObjectKey, string? VersionId), S3DeleteError> errors = new Dictionary<(string? ObjectKey, string? VersionId), S3DeleteError>();
        int operationErrors = 0;

        //Google and BackBlaze counts multipart uploads as part of bucket
        //Abort all incomplete multipart uploads
        IAsyncEnumerable<S3Upload> partUploads = client.ListAllMultipartUploadsAsync(bucket, token);

        await foreach (S3Upload partUpload in partUploads.WithCancellation(token))
        {
            AbortMultipartUploadResponse abortResp = await client.AbortMultipartUploadAsync(bucket, partUpload.ObjectKey, partUpload.UploadId, token: token);

            if (!abortResp.IsSuccess)
                operationErrors++;
        }

        //Try to delete all objects
        await foreach (S3DeleteError error in DeleteAllObjectVersionsAsync(provider, client, bucket, token).WithCancellation(token))
            errors[(error.ObjectKey, error.VersionId)] = error;

        if (provider == S3Provider.GoogleCloudStorage)
        {
            await foreach (S3Object obj in client.ListAllObjectsAsync(bucket, token: token).WithCancellation(token))
            {
                DeleteObjectResponse deleteResponse = await client.DeleteObjectAsync(bucket, obj.ObjectKey, token: token);

                if (!deleteResponse.IsSuccess)
                    errors[(obj.ObjectKey, null)] = new S3DeleteError(obj.ObjectKey, deleteResponse.Error?.Code ?? ErrorCode.Unknown, deleteResponse.Error?.Message ?? "Unable to delete object.", null);
            }
        }

        //If we have any errors at this point, it might be because of legal hold. Force delete them too.
        if (errors.Count > 0)
        {
            foreach (S3DeleteError error in errors.Values.ToArray()) // ToArray ensures stability since we mutate the dictionary
            {
                if (string.IsNullOrEmpty(error.ObjectKey))
                    continue;

                PutObjectLegalHoldResponse legalResp = await client.PutObjectLegalHoldAsync(bucket, error.ObjectKey, false, r => r.VersionId = error.VersionId, token);

                if (legalResp.IsSuccess)
                {
                    DeleteObjectResponse delResp = await client.DeleteObjectAsync(bucket, error.ObjectKey, request =>
                    {
                        request.VersionId = error.VersionId;
                        request.BypassGovernanceRetention = provider == S3Provider.AmazonS3;
                    }, token);

                    if (delResp.IsSuccess)
                        errors.Remove((error.ObjectKey, error.VersionId));
                }
            }
        }

        return operationErrors + errors.Count;
    }

    public static async Task<(Exception? Failure, bool Deferred)> TryDeleteTemporaryBucketAsync(S3Provider provider, ISimpleClient client, string bucket, int maxAttempts = 6, CancellationToken token = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxAttempts, 1);

        if (!IsTemporaryBucket(bucket))
            return (new ArgumentException("Only temporary buckets can be deleted by this operation.", nameof(bucket)), false);

        Exception? lastFailure = null;
        int lastEmptyErrors = -1;
        int? lastStatusCode = null;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int errors = -1;
            Exception? cleanupFailure = null;

            try
            {
                errors = await ForceEmptyBucketAsync(provider, client, bucket, token);
                lastEmptyErrors = errors;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                cleanupFailure = ex;
            }

            try
            {
                DeleteBucketResponse response = await client.DeleteBucketAsync(bucket, token: token);
                lastStatusCode = response.StatusCode;

                if (response.IsSuccess || response.StatusCode == 404)
                    return (null, false);

                string message = response.Error?.Message ?? "Unknown error";
                string cleanupMessage = cleanupFailure == null ? string.Empty : $" Cleanup failure: {cleanupFailure.Message}";
                lastFailure = new InvalidOperationException($"Unable to delete temporary bucket '{bucket}'. Status: {response.StatusCode}. Empty errors: {errors}. Message: {message}.{cleanupMessage}");
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

        lastFailure ??= new InvalidOperationException($"Unable to delete temporary bucket '{bucket}'.");
        bool deferred = provider == S3Provider.Wasabi && lastStatusCode == 424 && lastEmptyErrors == 0;
        return (lastFailure, deferred);
    }

    private static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersionsAsync(S3Provider provider, ISimpleClient client, string bucket, [EnumeratorCancellation]CancellationToken token = default)
    {
        Task<ListObjectVersionsResponse>? responseTask = null;
        CancellationTokenSource prefetchCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            responseTask = client.ListObjectVersionsAsync(bucket, token: prefetchCancellation.Token);

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

                if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                    break;

                if (response.IsTruncated)
                {
                    string? keyMarker = response.NextKeyMarker;
                    string? versionIdMarker = response.NextVersionIdMarker;
                    responseTask = client.ListObjectVersionsAsync(bucket, req =>
                    {
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
                    DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucket, delete, req => req.Quiet = false, prefetchCancellation.Token);

                    if (!multiDelResponse.IsSuccess)
                    {
                        foreach (S3DeleteInfo info in delete)
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
}