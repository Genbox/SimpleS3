using System;
using System.Globalization;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class TagTests : LiveTestBase
    {
        public TagTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task MultipleTags()
        {
            await UploadAsync(nameof(MultipleTags), request =>
            {
                request.Tags.Add("mykey1", "myvalue1");
                request.Tags.Add("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(MultipleTags)).ConfigureAwait(false);
            Assert.Equal(2, gResp.TagCount);
        }


        [Fact]
        public async Task MultipleTagsFluent()
        {
            await UploadTransferAsync(nameof(MultipleTagsFluent), upload =>
            {
                upload.WithTag("mykey1", "myvalue1");
                upload.WithTag("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(MultipleTagsFluent)).ConfigureAwait(false);
            Assert.Equal(2, gResp.TagCount);
        }

        [Fact]
        public async Task MultipleTagsOnHead()
        {
            await UploadAsync(nameof(MultipleTagsOnHead), request =>
            {
                request.Tags.Add("mykey1", "myvalue1");
                request.Tags.Add("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            HeadObjectResponse gResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(MultipleTagsOnHead)).ConfigureAwait(false);
            Assert.Equal(2, gResp.TagCount);
        }

        [Fact]
        public async Task TooManyTags()
        {
            await Assert.ThrowsAsync<Exception>(async () => await ObjectClient.PutObjectStringAsync(BucketName, nameof(TooManyTags), "data", null, request =>
            {
                for (int i = 0; i < 51; i++)
                    request.Tags.Add(i.ToString(NumberFormatInfo.InvariantInfo), i.ToString(NumberFormatInfo.InvariantInfo));
            }).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }
}