using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.PreSign
{
    public class PreSignedTests : OnlineTestBase
    {
        public PreSignedTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task FullPreSignTest()
        {
            int expireIn = 100;

            PutObjectRequest putReq = new PutObjectRequest(BucketName, "test.zip", null);
            string url = PreSignedObjectClient.SignPutObjectRequest(putReq, TimeSpan.FromSeconds(expireIn));

            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("hello world")))
            {
                PutObjectResponse? putResp = await PreSignedObjectClient.PutObjectAsync(url, ms).ConfigureAwait(false);
                Assert.Equal(200, putResp.StatusCode);
            }

            GetObjectRequest getReq = new GetObjectRequest(BucketName, "test.zip");
            url = PreSignedObjectClient.SignGetObjectRequest(getReq, TimeSpan.FromSeconds(expireIn));

            GetObjectResponse? getResp = await PreSignedObjectClient.GetObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);

            DeleteObjectRequest req = new DeleteObjectRequest(BucketName, "test.zip");
            url = PreSignedObjectClient.SignDeleteObjectRequest(req, TimeSpan.FromSeconds(expireIn));

            DeleteObjectResponse deleteResp = await PreSignedObjectClient.DeleteObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(204, deleteResp.StatusCode);

            HeadObjectRequest headReq = new HeadObjectRequest(BucketName, "test.zip");
            url = PreSignedObjectClient.SignHeadObjectRequest(headReq, TimeSpan.FromSeconds(expireIn));

            HeadObjectResponse headResp = await PreSignedObjectClient.HeadObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(404, headResp.StatusCode);
        }
    }
}