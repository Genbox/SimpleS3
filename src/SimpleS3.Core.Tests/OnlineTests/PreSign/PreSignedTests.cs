using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
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
            string url = PreSignedObjectOperations.SignPutObject(putReq, TimeSpan.FromSeconds(expireIn));

            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("hello world")))
            {
                (int putStatus, _, _) = await NetworkDriver.SendRequestAsync(HttpMethod.PUT, url, null, ms).ConfigureAwait(false);
                Assert.Equal(200, putStatus);
            }

            GetObjectRequest getReq = new GetObjectRequest(BucketName, "test.zip");
            url = PreSignedObjectOperations.SignGetObject(getReq, TimeSpan.FromSeconds(expireIn));

            (int getStatus, _, _) = await NetworkDriver.SendRequestAsync(HttpMethod.GET, url).ConfigureAwait(false);
            Assert.Equal(200, getStatus);

            DeleteObjectRequest req = new DeleteObjectRequest(BucketName, "test.zip");
            url = PreSignedObjectOperations.SignDeleteObject(req, TimeSpan.FromSeconds(expireIn));

            (int deleteStatus, _, _) = await NetworkDriver.SendRequestAsync(HttpMethod.DELETE, url).ConfigureAwait(false);
            Assert.Equal(204, deleteStatus);

            HeadObjectRequest headReq = new HeadObjectRequest(BucketName, "test.zip");
            url = PreSignedObjectOperations.SignHeadObject(headReq, TimeSpan.FromSeconds(expireIn));

            (int headStatus, _, _) = await NetworkDriver.SendRequestAsync(HttpMethod.HEAD, url).ConfigureAwait(false);
            Assert.Equal(404, headStatus);
        }
    }
}