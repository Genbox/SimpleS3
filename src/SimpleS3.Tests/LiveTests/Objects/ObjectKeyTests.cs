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
        [InlineData("/")]
        [InlineData(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~/")]
        public async Task TestValidCharacters(string name)
        {
            PutObjectResponse putResp = await ObjectClient.PutObjectStringAsync(BucketName, name, string.Empty).ConfigureAwait(false);
            Assert.True(putResp.IsSuccess);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, name).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);
        }

        [Theory]
        [InlineData(".")]
        [InlineData("\0")]
        public async Task TestInvalidCharacters(string name)
        {
            //These 2 test cases came after an exhaustive search in the whole UTF-16 character space.

            PutObjectResponse putResp = await ObjectClient.PutObjectStringAsync(BucketName, name, string.Empty).ConfigureAwait(false);
            Assert.False(putResp.IsSuccess);
        }
    }
}