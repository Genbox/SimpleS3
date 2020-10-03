using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Signed
{
    public class SignedTests : OnlineTestBase
    {
        public SignedTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task FullPreSignTest()
        {
            int expireIn = 100;

            string url = SignedObjectClient.SignPutObject(BucketName, "test.zip", null, TimeSpan.FromSeconds(expireIn));

            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("hello world")))
            {
                PutObjectResponse? putResp = await SignedObjectClient.PutObjectAsync(url, ms).ConfigureAwait(false);
                Assert.Equal(200, putResp.StatusCode);
            }

            url = SignedObjectClient.SignGetObject(BucketName, "test.zip", TimeSpan.FromSeconds(expireIn));

            GetObjectResponse? getResp = await SignedObjectClient.GetObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);

            url = SignedObjectClient.SignDeleteObject(BucketName, "test.zip", TimeSpan.FromSeconds(expireIn));

            DeleteObjectResponse deleteResp = await SignedObjectClient.DeleteObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(204, deleteResp.StatusCode);

            url = SignedObjectClient.SignHeadObject(BucketName, "test.zip", TimeSpan.FromSeconds(expireIn));

            HeadObjectResponse headResp = await SignedObjectClient.HeadObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(404, headResp.StatusCode);
        }
    }
}