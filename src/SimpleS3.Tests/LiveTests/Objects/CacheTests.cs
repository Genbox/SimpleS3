using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class CacheTests : LiveTestBase
    {
        public CacheTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task CacheControl()
        {
            await UploadAsync(nameof(CacheControl), req => req.CacheControl.Set(CacheControlType.MaxAge, 100)).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(CacheControl)).ConfigureAwait(false);
            Assert.Equal("max-age=100", gResp.CacheControl);
        }

        [Fact]
        public async Task CacheControlFluent()
        {
            await UploadTransferAsync(nameof(CacheControlFluent), upload => upload.WithCacheControl(CacheControlType.MaxAge, 100)).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(CacheControlFluent)).ConfigureAwait(false);
            Assert.Equal("max-age=100", gResp.CacheControl);
        }

        [Fact]
        public async Task IfETagMatch()
        {
            PutObjectResponse resp = await UploadAsync(nameof(IfETagMatch)).ConfigureAwait(false);

            await AssertAsync(nameof(IfETagMatch), req => req.IfETagMatch.Set(resp.ETag)).ConfigureAwait(false);

            GetObjectResponse resp2 = await AssertAsync(nameof(IfETagMatch), req => req.IfETagMatch.Set("not the tag you are looking for"), false).ConfigureAwait(false);
            Assert.Equal(412, resp2.StatusCode);
        }

        [Fact]
        public async Task IfETagMatchFluent()
        {
            PutObjectResponse resp = await UploadTransferAsync(nameof(IfETagMatchFluent)).ConfigureAwait(false);

            await AssertTransferAsync(nameof(IfETagMatchFluent), down => down.WithEtagConditional(resp.ETag)).ConfigureAwait(false);

            GetObjectResponse resp2 = await AssertTransferAsync(nameof(IfETagMatchFluent), down => down.WithEtagConditional("not the tag you are looking for"), false).ConfigureAwait(false);
            Assert.Equal(412, resp2.StatusCode);
        }

        [Fact]
        public async Task IfETagNotMatch()
        {
            PutObjectResponse resp = await UploadAsync(nameof(IfETagNotMatch)).ConfigureAwait(false);

            await AssertAsync(nameof(IfETagNotMatch), req => req.IfETagNotMatch.Set("not the tag you are looking for")).ConfigureAwait(false);

            GetObjectResponse resp2 = await AssertAsync(nameof(IfETagNotMatch), req => req.IfETagNotMatch.Set(resp.ETag), false).ConfigureAwait(false);
            Assert.Equal(304, resp2.StatusCode);
        }

        [Fact]
        public async Task IfETagNotMatchFluent()
        {
            PutObjectResponse resp = await UploadTransferAsync(nameof(IfETagNotMatchFluent)).ConfigureAwait(false);

            await AssertTransferAsync(nameof(IfETagNotMatchFluent), down => down.WithEtagConditional(null, "not the tag you are looking for")).ConfigureAwait(false);

            GetObjectResponse resp2 = await AssertTransferAsync(nameof(IfETagNotMatchFluent), down => down.WithEtagConditional(null, resp.ETag), false).ConfigureAwait(false);
            Assert.Equal(304, resp2.StatusCode);
        }
    }
}