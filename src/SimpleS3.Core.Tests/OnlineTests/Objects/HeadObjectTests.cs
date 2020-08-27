using System;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Objects
{
    public class HeadObjectTests : OnlineTestBase
    {
        public HeadObjectTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task HeadObject()
        {
            await UploadAsync(nameof(HeadObject)).ConfigureAwait(false);

            HeadObjectResponse gResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(HeadObject)).ConfigureAwait(false);
            Assert.Equal(200, gResp.StatusCode);
        }

        [Fact]
        public async Task HeadObjectContentRange()
        {
            await UploadAsync(nameof(HeadObjectContentRange)).ConfigureAwait(false);

            HeadObjectResponse headResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(HeadObjectContentRange), req => req.Range.Add(0, 2)).ConfigureAwait(false);
            Assert.Equal("bytes", headResp.AcceptRanges);
            Assert.Equal("bytes 0-2/4", headResp.ContentRange);
        }

        [Fact]
        public async Task HeadObjectLifecycle()
        {
            PutObjectResponse putResp = await UploadAsync(nameof(HeadObjectLifecycle)).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            HeadObjectResponse headResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(HeadObjectLifecycle)).ConfigureAwait(false);

            //Expiration should work on head too
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, headResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", headResp.LifeCycleRuleId);
        }

        [Fact]
        public async Task HeadObjectMultipleTags()
        {
            await UploadAsync(nameof(HeadObjectMultipleTags), request =>
            {
                request.Tags.Add("mykey1", "myvalue1");
                request.Tags.Add("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            HeadObjectResponse gResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(HeadObjectMultipleTags)).ConfigureAwait(false);
            Assert.Equal(2, gResp.TagCount);
        }

        [Fact]
        public async Task HeadObjectResponseHeaders()
        {
            //Upload a file for tests
            await UploadAsync(nameof(HeadObjectResponseHeaders)).ConfigureAwait(false);

            HeadObjectResponse response = await ObjectClient.HeadObjectAsync(BucketName, nameof(HeadObjectResponseHeaders), request =>
            {
                request.ResponseCacheControl.Set(CacheControlType.MaxAge, 42);
                request.ResponseContentDisposition.Set(ContentDispositionType.Attachment, "filename.txt");
                request.ResponseContentEncoding.Add(ContentEncodingType.Gzip);
                request.ResponseContentLanguage.Add("da-DK");
                request.ResponseContentType.Set("text/html", "utf-8");
                request.ResponseExpires = DateTimeOffset.UtcNow;
            }).ConfigureAwait(false);

            Assert.Equal("max-age=42", response.CacheControl);
            Assert.Equal("attachment; filename*=\"filename.txt\"", response.ContentDisposition);
            Assert.Equal("da-DK", response.ContentLanguage);
            Assert.Equal("text/html; charset=utf-8", response.ContentType);
            Assert.Equal(DateTime.UtcNow, response.ExpiresOn!.Value.DateTime, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task HeadObjectWebsiteRedirect()
        {
            await UploadAsync(nameof(HeadObjectWebsiteRedirect), request => request.WebsiteRedirectLocation = "https://google.com").ConfigureAwait(false);

            HeadObjectResponse resp1 = await ObjectClient.HeadObjectAsync(BucketName, nameof(HeadObjectWebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal("https://google.com", resp1.WebsiteRedirectLocation);
            Assert.Equal(200, resp1.StatusCode);
        }
    }
}