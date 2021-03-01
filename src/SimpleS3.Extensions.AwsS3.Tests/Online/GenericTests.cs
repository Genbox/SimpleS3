using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online
{
    public class GenericTests : AwsTestBase
    {
        public GenericTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task SimpleTest()
        {
            await CreateTempBucketAsync(async name =>
            {
                string objectKey = "object-key";

                PutObjectResponse? putResp = await ObjectClient.PutObjectAsync(name, objectKey, null).ConfigureAwait(false);
                await ObjectClient.GetObjectAsync(name, objectKey).ConfigureAwait(false);
                await ObjectClient.DeleteObjectAsync(name, objectKey, x => x.VersionId = putResp.VersionId).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }
}