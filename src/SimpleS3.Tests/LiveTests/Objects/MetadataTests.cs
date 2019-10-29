using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class MetadataTests : LiveTestBase
    {
        public MetadataTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task LargeMetadata()
        {
            string value = new string('b', 2047);

            await UploadAsync(nameof(MultipleMetadata), request =>
            {
                //Amazon ignores the metadata prefix and header separator, so we just need to count key length + value length
                request.Metadata.Add("a", value);
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(MultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(value, gResp.Metadata["a"]);
        }

        [Fact]
        public async Task MultipleMetadata()
        {
            await UploadAsync(nameof(MultipleMetadata), request =>
            {
                for (int i = 0; i < 10; i++)
                    request.Metadata.Add("mykey" + i, "myvalue" + i);
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(MultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(10, gResp.Metadata.Count);
        }


        [Fact]
        public async Task MultipleMetadataFluent()
        {
            await UploadTransferAsync(nameof(MultipleMetadata), upload =>
            {
                for (int i = 0; i < 10; i++)
                    upload.WithMetadata("mykey" + i, "myvalue" + i);
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(MultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(10, gResp.Metadata.Count);
        }

        [Fact]
        public async Task SingleMetadata()
        {
            await UploadAsync(nameof(SingleMetadata), request => { request.Metadata.Add("mykey", "myvalue"); }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(SingleMetadata)).ConfigureAwait(false);
            Assert.Equal("myvalue", gResp.Metadata["mykey"]);
        }

        [Fact]
        public Task SpecialCharacters()
        {
            return UploadAsync(nameof(MultipleMetadata), request => request.Metadata.Add("a", "!\" #$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"));
        }
    }
}