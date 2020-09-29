using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Multipart
{
    public class AbortMultipartTests : OnlineTestBase
    {
        public AbortMultipartTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task AbortIncompleteUpload()
        {
            string objectKey = nameof(AbortIncompleteUpload);

            CreateMultipartUploadResponse createResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey).ConfigureAwait(false);

            Assert.Equal(BucketName, createResp.Bucket);
            Assert.Equal(objectKey, createResp.ObjectKey);
            Assert.NotNull(createResp.UploadId);

            AbortMultipartUploadResponse abortResp = await MultipartClient.AbortMultipartUploadAsync(BucketName, objectKey, createResp.UploadId).ConfigureAwait(false);

            Assert.True(abortResp.IsSuccess);
            Assert.Equal(204, abortResp.StatusCode);
        }
    }
}