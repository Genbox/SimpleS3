using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Signed
{
    public class SignedTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task FullPreSignTest(S3Provider _, string bucket, ISimpleClient client)
        {
            int expireIn = 100;

            string url = client.SignPutObject(bucket, "test.zip", null, TimeSpan.FromSeconds(expireIn));

            await using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes("hello world")))
            {
                PutObjectResponse putResp = await client.PutObjectAsync(url, ms).ConfigureAwait(false);
                Assert.Equal(200, putResp.StatusCode);
            }

            url = client.SignGetObject(bucket, "test.zip", TimeSpan.FromSeconds(expireIn));

            GetObjectResponse getResp = await client.GetObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);

            url = client.SignDeleteObject(bucket, "test.zip", TimeSpan.FromSeconds(expireIn));

            DeleteObjectResponse deleteResp = await client.DeleteObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(204, deleteResp.StatusCode);

            url = client.SignHeadObject(bucket, "test.zip", TimeSpan.FromSeconds(expireIn));

            HeadObjectResponse headResp = await client.HeadObjectAsync(url).ConfigureAwait(false);
            Assert.Equal(404, headResp.StatusCode);
        }
    }
}