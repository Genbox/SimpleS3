using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Multipart
{
    public class AbortMultipartTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task AbortIncompleteUpload(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string objectKey = nameof(AbortIncompleteUpload);
            string bucketName = GetTestBucket(profile);

            CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucketName, objectKey).ConfigureAwait(false);

            Assert.Equal(bucketName, createResp.BucketName);
            Assert.Equal(objectKey, createResp.ObjectKey);
            Assert.NotNull(createResp.UploadId);

            AbortMultipartUploadResponse abortResp = await client.AbortMultipartUploadAsync(bucketName, objectKey, createResp.UploadId).ConfigureAwait(false);

            Assert.True(abortResp.IsSuccess);
            Assert.Equal(204, abortResp.StatusCode);
        }
    }
}