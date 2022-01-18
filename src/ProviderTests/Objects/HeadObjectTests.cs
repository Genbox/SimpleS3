using System;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class HeadObjectTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadObject(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(HeadObject), null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucket, nameof(HeadObject)).ConfigureAwait(false);
            Assert.Equal(200, headResp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task HeadObjectContentRange(S3Provider _, string bucket, ISimpleClient client)
        {
            await client.PutObjectStringAsync(bucket, nameof(HeadObjectContentRange), "test").ConfigureAwait(false);

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucket, nameof(HeadObjectContentRange), r => r.Range.Add(0, 2)).ConfigureAwait(false);
            Assert.Equal("bytes", headResp.AcceptRanges);
            Assert.Equal("bytes 0-2/4", headResp.ContentRange);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task HeadObjectLifecycle(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(HeadObjectLifecycle), null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucket, nameof(HeadObjectLifecycle)).ConfigureAwait(false);

            //Expiration should work on head too
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, headResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", headResp.LifeCycleRuleId);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task HeadObjectMultipleTags(S3Provider _, string bucket, ISimpleClient client)
        {
            await client.PutObjectAsync(bucket, nameof(HeadObjectMultipleTags), null, r =>
            {
                r.Tags.Add("mykey1", "myvalue1");
                r.Tags.Add("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            HeadObjectResponse resp = await client.HeadObjectAsync(bucket, nameof(HeadObjectMultipleTags)).ConfigureAwait(false);
            Assert.Equal(2, resp.TagCount);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
        public async Task HeadObjectResponseHeaders(S3Provider _, string bucket, ISimpleClient client)
        {
            //Upload a file for tests
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(HeadObjectResponseHeaders), null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            HeadObjectResponse resp = await client.HeadObjectAsync(bucket, nameof(HeadObjectResponseHeaders), r =>
            {
                r.ResponseCacheControl.Set(CacheControlType.MaxAge, 42);
                r.ResponseContentDisposition.Set(ContentDispositionType.Attachment, "filename.txt");
                r.ResponseContentEncoding.Add(ContentEncodingType.Gzip);
                r.ResponseContentLanguage.Add("da-DK");
                r.ResponseContentType.Set("text/html", "utf-8");
                r.ResponseExpires = DateTimeOffset.UtcNow;
            }).ConfigureAwait(false);

            Assert.Equal("max-age=42", resp.CacheControl);
            Assert.Equal("attachment; filename=\"filename.txt\"", resp.ContentDisposition);
            Assert.Equal("da-DK", resp.ContentLanguage);
            Assert.Equal("text/html; charset=utf-8", resp.ContentType);
            Assert.Equal(DateTime.UtcNow, resp.ExpiresOn!.Value.DateTime, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task HeadObjectWebsiteRedirect(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(HeadObjectWebsiteRedirect), null, r => r.WebsiteRedirectLocation = "https://google.com").ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucket, nameof(HeadObjectWebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal(200, headResp.StatusCode);
            Assert.Equal("https://google.com", headResp.WebsiteRedirectLocation);
        }
    }
}