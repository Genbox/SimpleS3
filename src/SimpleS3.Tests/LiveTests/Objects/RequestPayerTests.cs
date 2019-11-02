using System.IO;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class RequestPayerTests : LiveTestBase
    {
        public RequestPayerTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task PutGetRequestPayerTest()
        {
            PutObjectResponse putResp = await ObjectClient.PutObjectAsync(BucketName, nameof(PutGetRequestPayerTest), null, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(putResp.RequestCharged);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, nameof(PutGetRequestPayerTest), req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(getResp.RequestCharged);
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task MultipartRequestPayerTest()
        {
            string objectKey = nameof(MultipartRequestPayerTest);

            CreateMultipartUploadResponse initResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(initResp.RequestCharged);

            byte[] file = new byte[1024 * 1024 * 5];

            UploadPartResponse uploadResp = await MultipartClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, new MemoryStream(file), req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(uploadResp.RequestCharged);

            ListPartsResponse listResp = await MultipartClient.ListPartsAsync(BucketName, objectKey, initResp.UploadId, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(listResp.RequestCharged);

            CompleteMultipartUploadResponse completeResp = await MultipartClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp }, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(completeResp.RequestCharged);
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task ListObjectsRequestPayerTest()
        {
            ListObjectsResponse listResp = await ObjectClient.ListObjectsAsync(BucketName, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(listResp.RequestCharged);
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task DeleteRequestPayerTest()
        {
            PutObjectResponse putResp = await ObjectClient.PutObjectAsync(BucketName, nameof(DeleteRequestPayerTest), null, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(putResp.RequestCharged);

            DeleteObjectResponse delResp = await ObjectClient.DeleteObjectAsync(BucketName, nameof(DeleteRequestPayerTest), req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(delResp.RequestCharged);

            PutObjectResponse putResp2 = await ObjectClient.PutObjectAsync(BucketName, nameof(DeleteRequestPayerTest), null, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(putResp2.RequestCharged);

            DeleteObjectsResponse delResp2 = await ObjectClient.DeleteObjectsAsync(BucketName, new[] { nameof(DeleteRequestPayerTest) }, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(delResp2.RequestCharged);
        }
    }
}
