using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class RangeTests : LiveTestBase
    {
        public RangeTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task Range()
        {
            await ObjectClient.PutObjectStringAsync(BucketName, nameof(Range), "123456789012345678901234567890123456789012345678901234567890").ConfigureAwait(false);

            GetObjectResponse resp = await ObjectClient.GetObjectAsync(BucketName, nameof(Range), request => request.Range.Add(0, 10)).ConfigureAwait(false);
            Assert.Equal(206, resp.StatusCode);
            Assert.Equal(11, resp.ContentLength);
        }

        [Fact]
        public async Task RangeFluent()
        {
            await ObjectClient.PutObjectStringAsync(BucketName, nameof(RangeFluent), "123456789012345678901234567890123456789012345678901234567890").ConfigureAwait(false);

            GetObjectResponse resp = await Transfer.Download(BucketName, nameof(RangeFluent))
                .WithRange(0, 10)
                .ExecuteAsync()
                .ConfigureAwait(false);

            Assert.Equal(206, resp.StatusCode);
            Assert.Equal(11, resp.ContentLength);
        }
    }
}