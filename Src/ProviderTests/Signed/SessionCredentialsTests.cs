using System.Globalization;
using System.Xml.Linq;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.AmazonS3;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.ProviderTests.Signed;

public class SessionCredentialsTests : TestBase
{
    private const string StsService = "sts";
    private const string StsQuery = "Action=GetSessionToken&DurationSeconds=900&Version=2011-06-15";
    private const string EmptyPayloadHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task SessionCredentialsCanAccessBucket(S3Provider _1, string bucket, ISimpleClient _2)
    {
        IProfile profile = ProviderSetup.Instance.GetProfile(S3Provider.AmazonS3);
        byte[] secretKey = ProviderSetup.Instance.GetAccessKeyProtector(S3Provider.AmazonS3).UnprotectKey(profile.AccessKey);

        try
        {
            await using ServiceProvider signingProvider = CreateSigningProvider(profile.KeyId, secretKey);
            IAuthorizationBuilder authorizationBuilder = signingProvider.GetRequiredService<IAuthorizationBuilder>();
            ListBucketsRequest signingRequest = new ListBucketsRequest
            {
                Timestamp = DateTimeOffset.UtcNow
            };
            signingRequest.SetQueryParameter("Action", "GetSessionToken");
            signingRequest.SetQueryParameter("DurationSeconds", "900");
            signingRequest.SetQueryParameter("Version", "2011-06-15");
            signingRequest.SetHeader(HttpHeaders.Host, "sts.amazonaws.com");
            signingRequest.SetHeader(AmzHeaders.XAmzDate, signingRequest.Timestamp.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture));
            signingRequest.SetHeader(AmzHeaders.XAmzContentSha256, EmptyPayloadHash);
            authorizationBuilder.BuildAuthorization(signingRequest, StsService);

            using HttpClient httpClient = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://sts.amazonaws.com/?" + StsQuery);

            foreach ((string name, string value) in signingRequest.Headers)
            {
                if (name == HttpHeaders.Host)
                    request.Headers.Host = value;
                else
                    request.Headers.TryAddWithoutValidation(name, value);
            }

            using HttpResponseMessage response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);
            response.EnsureSuccessStatusCode();

            string responseXml = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            XNamespace ns = "https://sts.amazonaws.com/doc/2011-06-15/";
            XElement credentialsElement = XDocument.Parse(responseXml).Root!.Element(ns + "GetSessionTokenResult")!.Element(ns + "Credentials")!;

            string keyId = credentialsElement.Element(ns + "AccessKeyId")!.Value;
            string sessionSecretKey = credentialsElement.Element(ns + "SecretAccessKey")!.Value;
            string sessionToken = credentialsElement.Element(ns + "SessionToken")!.Value;

            using AmazonS3Client client = new AmazonS3Client(new AmazonS3Config(new StringSessionAccessKey(keyId, sessionSecretKey, sessionToken), profile.RegionCode));
            HeadBucketResponse headResponse = await client.HeadBucketAsync(bucket, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, headResponse.StatusCode);
        }
        finally
        {
            Array.Clear(secretKey);
        }
    }

    private static ServiceProvider CreateSigningProvider(string keyId, byte[] secretKey)
    {
        ServiceCollection services = new ServiceCollection();
        SimpleS3CoreServices.AddSimpleS3Core(services, config =>
        {
            config.Endpoint = "https://sts.amazonaws.com";
            config.NamingMode = NamingMode.PathStyle;
            config.RegionCode = "us-east-1";
            config.Credentials = new AccessKey(keyId, secretKey);
        });
        return services.BuildServiceProvider();
    }
}