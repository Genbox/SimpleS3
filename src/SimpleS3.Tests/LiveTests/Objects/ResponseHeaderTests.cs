using System;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class ResponseHeaderTests : LiveTestBase
    {
        public ResponseHeaderTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task ResponseHeaders()
        {
            //Upload a file for tests
            await UploadAsync(nameof(ResponseHeaders)).ConfigureAwait(false);

            GetObjectResponse response = await ObjectClient.GetObjectAsync(BucketName, nameof(ResponseHeaders), request =>
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
            Assert.Equal("gzip", response.ContentEncoding);
            Assert.Equal("da-DK", response.ContentLanguage);
            Assert.Equal("text/html; charset=utf-8", response.ContentType);
            Assert.Equal(DateTime.UtcNow, response.Expires.Value.DateTime, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task ResponseHeadersOnHead()
        {
            //Upload a file for tests
            await UploadAsync(nameof(ResponseHeadersOnHead)).ConfigureAwait(false);

            HeadObjectResponse response = await ObjectClient.HeadObjectAsync(BucketName, nameof(ResponseHeadersOnHead), request =>
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
            Assert.Equal(DateTime.UtcNow, response.Expires.Value.DateTime, TimeSpan.FromSeconds(5));
        }
    }
}