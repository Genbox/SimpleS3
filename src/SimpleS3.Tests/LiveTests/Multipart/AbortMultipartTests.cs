using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Multipart
{
    public class AbortMultipartTests : LiveTestBase
    {
        public AbortMultipartTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task AbortIncompleteUpload()
        {
            string objectKey = nameof(AbortIncompleteUpload);

            CreateMultipartUploadResponse initResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey).ConfigureAwait(false);

            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(objectKey, initResp.ObjectKey);
            Assert.NotNull(initResp.UploadId);

            AbortMultipartUploadResponse abortResp = await MultipartClient.AbortMultipartUploadAsync(BucketName, objectKey, initResp.UploadId).ConfigureAwait(false);

            Assert.True(abortResp.IsSuccess);
            Assert.Equal(204, abortResp.StatusCode);
        }
    }
}