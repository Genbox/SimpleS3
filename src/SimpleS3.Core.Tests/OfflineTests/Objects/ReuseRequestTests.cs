using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests.Objects
{
    public class ReuseRequestTests : OfflineTestBase
    {
        public ReuseRequestTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task ReuseRequest()
        {
            GetObjectRequest req = new GetObjectRequest("testbucket", "testobject");
            req.PartNumber = 5;
            req.VersionId = "versionid";

            for (int i = 0; i < 10; i++)
            {
                GetObjectResponse resp = await ObjectClient.ObjectOperations.GetObjectAsync(req).ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                //None of the essential properties must change
                Assert.Equal("testbucket", req.BucketName);
                Assert.Equal("testobject", req.ObjectKey);

                //None of the user supplied properties must change
                Assert.Equal(5, req.PartNumber);
                Assert.Equal("versionid", req.VersionId);
            }
        }
    }
}
