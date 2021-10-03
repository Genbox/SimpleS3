using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests
{
    public sealed class MultipleProvidersAttribute : Xunit.Sdk.DataAttribute
    {
        private ISimpleClient? _currentClient = null;

        public override IEnumerable<object?[]> GetData(System.Reflection.MethodInfo testMethod)
        {
            foreach (var (profile, client) in ProviderSetup.Instance.Clients)
            {
                yield return new object?[] { profile, client };
            }
        }

        public override string? Skip => /*_currentClient == null ? "not supported" : */null;
    }

    public class GetObjectTests
    {
        [Theory]
        [MultipleProviders]
        public async Task GetObjectContentRange(IProfile profile, ISimpleClient client)
        {
            string bucketName = UtilityHelper.GetTestBucket(profile);

            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, nameof(GetObjectContentRange), null);
            GetObjectResponse getResp = await client.GetObjectAsync(bucketName, nameof(GetObjectContentRange), req => req.Range.Add(0, 2)).ConfigureAwait(false);

            Assert.Equal(206, getResp.StatusCode);
            Assert.Equal(3, getResp.ContentLength);
            Assert.Equal("bytes", getResp.AcceptRanges);
            Assert.Equal("bytes 0-2/4", getResp.ContentRange);
            Assert.Equal("tes", await getResp.Content.AsStringAsync().ConfigureAwait(false));
        }
    }
}