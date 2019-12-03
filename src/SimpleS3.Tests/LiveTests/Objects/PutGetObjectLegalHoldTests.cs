using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class PutGetObjectLegalHoldTests : LiveTestBase
    {
        public PutGetObjectLegalHoldTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task PutGetObjectLegalHold()
        {
            string objectKey = nameof(PutGetObjectLegalHold);

            //Create an object
            await UploadAsync(objectKey).ConfigureAwait(false);

            //Check that there is no lock
            GetObjectLegalHoldResponse getLegalResp = await ObjectClient.GetObjectLegalHoldAsync(BucketName, objectKey).ConfigureAwait(false);
            Assert.Equal(404, getLegalResp.StatusCode);
            Assert.False(getLegalResp.LegalHold);

            //Set a lock
            PutObjectLegalHoldResponse putLegalResp = await ObjectClient.PutObjectLegalHoldAsync(BucketName, objectKey, true).ConfigureAwait(false);
            Assert.Equal(200, putLegalResp.StatusCode);

            //There should be a lock now
            GetObjectLegalHoldResponse getLegalResp2 = await ObjectClient.GetObjectLegalHoldAsync(BucketName, objectKey).ConfigureAwait(false);
            Assert.Equal(200, getLegalResp2.StatusCode);
            Assert.True(getLegalResp2.LegalHold);
        }
    }
}