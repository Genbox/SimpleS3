using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Objects
{
    public class DeleteObjectTests : OnlineTestBase
    {
        public DeleteObjectTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task DeleteObject()
        {
            await UploadAsync(nameof(DeleteObject)).ConfigureAwait(false);
            DeleteObjectResponse resp = await ObjectClient.DeleteObjectAsync(BucketName, nameof(DeleteObject)).ConfigureAwait(false);

            Assert.True(resp.IsDeleteMarker);
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task DeleteObjectRequestPayer()
        {
            PutObjectResponse putResp = await ObjectClient.PutObjectAsync(BucketName, nameof(DeleteObjectRequestPayer), null, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(putResp.RequestCharged);

            DeleteObjectResponse delResp = await ObjectClient.DeleteObjectAsync(BucketName, nameof(DeleteObjectRequestPayer), req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(delResp.RequestCharged);
        }
    }
}