using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class ObjectKeyTests : LiveTestBase
    {
        public ObjectKeyTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Theory]
        [InlineData("NormalFile")]
        [InlineData("This/Should/Look/Like/Directories/File.txt")]
        [InlineData("_\\_")]
        [InlineData("~")]
        [InlineData(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~/")]
        public async Task NameTests(string name)
        {
            await ObjectClient.PutObjectStringAsync(BucketName, name, name).ConfigureAwait(false);

            GetObjectResponse dlResp = await ObjectClient.GetObjectAsync(BucketName, name).ConfigureAwait(false);
            Assert.Equal("binary/octet-stream", dlResp.ContentType);
            Assert.Equal(name.Length, dlResp.ContentLength);

            Assert.Equal(name, Encoding.UTF8.GetString(await dlResp.Content.AsDataAsync().ConfigureAwait(false)));
        }
    }
}