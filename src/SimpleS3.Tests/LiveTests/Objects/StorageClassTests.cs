using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class StorageClassTests : LiveTestBase
    {
        public StorageClassTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Theory]
        [InlineData(StorageClass.Standard, true)]
        [InlineData(StorageClass.DeepArchive, false)]
        [InlineData(StorageClass.Glacier, false)]
        [InlineData(StorageClass.IntelligentTiering, true)]
        [InlineData(StorageClass.OneZoneIa, true)]
        [InlineData(StorageClass.ReducedRedundancy, true)]
        [InlineData(StorageClass.StandardIa, true)]
        public async Task CanUploadClass(StorageClass storageClass, bool canDownload)
        {
            PutObjectResponse pResp = await UploadAsync(nameof(CanUploadClass) + "-" + storageClass, request => request.StorageClass = storageClass).ConfigureAwait(false);
            Assert.Equal(storageClass, pResp.StorageClass);

            if (canDownload)
            {
                GetObjectResponse gResp = await AssertAsync(storageClass.ToString()).ConfigureAwait(false);
                Assert.Equal(storageClass, gResp.StorageClass);
            }
        }

        [Theory]
        [InlineData(StorageClass.Standard, true)]
        [InlineData(StorageClass.DeepArchive, false)]
        [InlineData(StorageClass.Glacier, false)]
        [InlineData(StorageClass.IntelligentTiering, true)]
        [InlineData(StorageClass.OneZoneIa, true)]
        [InlineData(StorageClass.ReducedRedundancy, true)]
        [InlineData(StorageClass.StandardIa, true)]
        public async Task CanUploadClassFluid(StorageClass storageClass, bool canDownload)
        {
            PutObjectResponse pResp = await UploadTransferAsync(nameof(CanUploadClassFluid) + "-" + storageClass, upload => upload.WithStorageClass(storageClass)).ConfigureAwait(false);
            Assert.Equal(storageClass, pResp.StorageClass);

            if (canDownload)
            {
                GetObjectResponse gResp = await AssertAsync(storageClass.ToString()).ConfigureAwait(false);
                Assert.Equal(storageClass, gResp.StorageClass);
            }
        }
    }
}