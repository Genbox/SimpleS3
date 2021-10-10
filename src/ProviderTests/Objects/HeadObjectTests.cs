using System;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class HeadObjectTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadObject(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(HeadObject), null).ConfigureAwait(false);

            HeadObjectResponse resp = await client.HeadObjectAsync(bucketName, nameof(HeadObject)).ConfigureAwait(false);
            Assert.Equal(200, resp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadObjectContentRange(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(HeadObjectContentRange), null).ConfigureAwait(false);

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucketName, nameof(HeadObjectContentRange), req => req.Range.Add(0, 2)).ConfigureAwait(false);
            Assert.Equal("bytes", headResp.AcceptRanges);
            Assert.Equal("bytes 0-2/4", headResp.ContentRange);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadObjectLifecycle(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, nameof(HeadObjectLifecycle), null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucketName, nameof(HeadObjectLifecycle)).ConfigureAwait(false);

            //Expiration should work on head too
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, headResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", headResp.LifeCycleRuleId);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadObjectMultipleTags(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(HeadObjectMultipleTags), null, request =>
            {
                request.Tags.Add("mykey1", "myvalue1");
                request.Tags.Add("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            HeadObjectResponse resp = await client.HeadObjectAsync(bucketName, nameof(HeadObjectMultipleTags)).ConfigureAwait(false);
            Assert.Equal(2, resp.TagCount);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadObjectResponseHeaders(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            //Upload a file for tests
            await client.PutObjectAsync(bucketName, nameof(HeadObjectResponseHeaders), null).ConfigureAwait(false);

            HeadObjectResponse resp = await client.HeadObjectAsync(bucketName, nameof(HeadObjectResponseHeaders), request =>
            {
                request.ResponseCacheControl.Set(CacheControlType.MaxAge, 42);
                request.ResponseContentDisposition.Set(ContentDispositionType.Attachment, "filename.txt");
                request.ResponseContentEncoding.Add(ContentEncodingType.Gzip);
                request.ResponseContentLanguage.Add("da-DK");
                request.ResponseContentType.Set("text/html", "utf-8");
                request.ResponseExpires = DateTimeOffset.UtcNow;
            }).ConfigureAwait(false);

            Assert.Equal("max-age=42", resp.CacheControl);
            Assert.Equal("attachment; filename=\"filename.txt\"", resp.ContentDisposition);
            Assert.Equal("da-DK", resp.ContentLanguage);
            Assert.Equal("text/html; charset=utf-8", resp.ContentType);
            Assert.Equal(DateTime.UtcNow, resp.ExpiresOn!.Value.DateTime, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadObjectWebsiteRedirect(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(HeadObjectWebsiteRedirect), null, request => request.WebsiteRedirectLocation = "https://google.com").ConfigureAwait(false);

            HeadObjectResponse resp = await client.HeadObjectAsync(bucketName, nameof(HeadObjectWebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal(200, resp.StatusCode);
            Assert.Equal("https://google.com", resp.WebsiteRedirectLocation);
        }
    }
}