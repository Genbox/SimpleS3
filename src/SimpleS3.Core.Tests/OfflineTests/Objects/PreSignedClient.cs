using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests.Objects
{
    public class PreSignedClient : OfflineTestBase
    {
        public PreSignedClient(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task UseClient()
        {
            GetObjectRequest req = new GetObjectRequest("mybucket", "myobject");
            req.ResponseContentDisposition.Set(ContentDispositionType.Attachment, "test.zip");

            string url = await PreSignedObjectOperations.SignGetObjectAsync(req).ConfigureAwait(false);

        }
    }
}
