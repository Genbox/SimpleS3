using System.Globalization;
using System.Net;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
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

                if (!int.TryParse(c, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int intVal) || intVal < 0 || intVal > choices.Length)
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

    public static bool IsTemporaryBucket(string bucketName) => Guid.TryParse(bucketName, out _) || bucketName.StartsWith("tempbucket-", StringComparison.OrdinalIgnoreCase);

    public static ServiceProvider CreateSimpleS3(S3Provider provider, string profileName, bool enableRetry, Action<SimpleS3Config>? configure = null)
    {
        ServiceCollection services = new ServiceCollection();
        ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services, configure);

        IConfigurationRoot configRoot = new ConfigurationBuilder()
                                        .SetBasePath(Environment.CurrentDirectory)
                                        .AddJsonFile("Config.json", false)
                                        .Build();

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

    public static async Task<int> ForceEmptyBucketAsync(S3Provider provider, ISimpleClient client, string bucket)
    {
        HashSet<S3DeleteError> errors = new HashSet<S3DeleteError>(ErrorComparer.Instance);

        //Google and BackBlaze counts multipart uploads as part of bucket
        //Abort all incomplete multipart uploads
        IAsyncEnumerable<S3Upload> partUploads = client.ListAllMultipartUploadsAsync(bucket);

        await foreach (S3Upload partUpload in partUploads)
        {
            AbortMultipartUploadResponse abortResp = await client.AbortMultipartUploadAsync(bucket, partUpload.ObjectKey, partUpload.UploadId);

            if (!abortResp.IsSuccess)
                errors.Add(new S3DeleteError(partUpload.ObjectKey, ErrorCode.Unknown, string.Empty, null));
        }

        //Try to delete all objects
        await foreach (S3DeleteError error in DeleteAllObjectVersions(provider, client, bucket))
            errors.Add(error);

        //If we have any errors at this point, it might be because of legal hold. Force delete them too.
        if (errors.Count > 0)
        {
            foreach (S3DeleteError error in errors)
            {
                PutObjectLegalHoldResponse legalResp = await client.PutObjectLegalHoldAsync(bucket, error.ObjectKey, false, r => r.VersionId = error.VersionId);

                if (legalResp.IsSuccess)
                {
                    DeleteObjectResponse delResp = await client.DeleteObjectAsync(bucket, error.ObjectKey, x => x.VersionId = error.VersionId);

                    if (delResp.IsSuccess)
                        errors.Remove(error);
                }
            }
        }

        return errors.Count;
    }

    private static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersions(S3Provider provider, ISimpleClient client, string bucket)
    {
        ListObjectVersionsResponse response;
        Task<ListObjectVersionsResponse> responseTask = client.ListObjectVersionsAsync(bucket);

        do
        {
            response = await responseTask;

            if (!response.IsSuccess)
                yield break;

            if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                break;

            if (response.IsTruncated)
            {
                string keyMarker = response.NextKeyMarker;
                responseTask = client.ListObjectVersionsAsync(bucket, req => req.KeyMarker = keyMarker);
            }

            IEnumerable<S3DeleteInfo> delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                       .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)));

            //Google does not support DeleteObjects
            if (provider == S3Provider.GoogleCloudStorage)
            {
                foreach (S3DeleteInfo info in delete)
                    await client.DeleteObjectAsync(bucket, info.ObjectKey, info.VersionId);
            }
            else
            {
                DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucket, delete, req => req.Quiet = false).ConfigureAwait(false);

                if (!multiDelResponse.IsSuccess)
                    yield break;

                foreach (S3DeleteError error in multiDelResponse.Errors)
                    yield return error;
            }
        } while (response.IsTruncated);
    }

    private sealed class ErrorComparer : IEqualityComparer<S3DeleteError>
    {
        public static readonly ErrorComparer Instance = new ErrorComparer();
        private ErrorComparer() {}

        public bool Equals(S3DeleteError? x, S3DeleteError? y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (ReferenceEquals(x, null))
                return false;
            if (ReferenceEquals(y, null))
                return false;
            if (x.GetType() != y.GetType())
                return false;
            return x.ObjectKey == y.ObjectKey && x.VersionId == y.VersionId;
        }

        public int GetHashCode(S3DeleteError obj) => HashCode.Combine(obj.ObjectKey, obj.VersionId);
    }
}