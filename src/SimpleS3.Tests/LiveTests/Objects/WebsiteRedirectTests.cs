using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class WebsiteRedirectTests : LiveTestBase
    {
        public WebsiteRedirectTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task WebsiteRedirect()
        {
            await UploadAsync(nameof(WebsiteRedirect), request => request.WebsiteRedirectLocation = "https://google.com").ConfigureAwait(false);

            GetObjectResponse resp1 = await AssertAsync(nameof(WebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal("https://google.com", resp1.WebsiteRedirectLocation);
            Assert.Equal(200, resp1.StatusCode);
        }

        [Fact]
        public async Task WebsiteRedirectFluid()
        {
            await UploadTransferAsync(nameof(WebsiteRedirectFluid), upload => upload.WithWebsiteRedirectLocation("https://google.com")).ConfigureAwait(false);

            GetObjectResponse resp1 = await AssertAsync(nameof(WebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal("https://google.com", resp1.WebsiteRedirectLocation);
            Assert.Equal(200, resp1.StatusCode);
        }

        [Fact]
        public async Task WebsiteRedirectOnHead()
        {
            await UploadAsync(nameof(WebsiteRedirectOnHead), request => request.WebsiteRedirectLocation = "https://google.com").ConfigureAwait(false);

            HeadObjectResponse resp1 = await ObjectClient.HeadObjectAsync(BucketName, nameof(WebsiteRedirectOnHead)).ConfigureAwait(false);
            Assert.Equal("https://google.com", resp1.WebsiteRedirectLocation);
            Assert.Equal(200, resp1.StatusCode);
        }
    }
}