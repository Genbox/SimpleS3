using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class GetObjectTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task GetObjectData(S3Provider _, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            // Object content should:
            // - Support any combination of bytes
            // - Preserve length

            byte[] binaryData = new byte[10];
            RandomNumberGenerator.Fill(binaryData);

            PutObjectResponse putResp1 = await client.PutObjectDataAsync(bucketName, nameof(GetObjectData), binaryData).ConfigureAwait(false);
            Assert.Equal(200, putResp1.StatusCode);

            GetObjectResponse getResp1 = await client.GetObjectAsync(bucketName, nameof(GetObjectData)).ConfigureAwait(false);
            Assert.Equal(200, getResp1.StatusCode);
            Assert.Equal(binaryData, await getResp1.Content.AsDataAsync());
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task GetObjectString(S3Provider _, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            // For strings it should:
            // - Preserve casing
            // - Preserve encoding

            string stringData = "Hello 你好 ਸਤ ਸ੍ਰੀ ਅਕਾਲ Привет";

            PutObjectResponse putResp2 = await client.PutObjectStringAsync(bucketName, nameof(GetObjectString), stringData).ConfigureAwait(false);
            Assert.Equal(200, putResp2.StatusCode);

            GetObjectResponse getResp2 = await client.GetObjectAsync(bucketName, nameof(GetObjectString)).ConfigureAwait(false);
            Assert.Equal(200, getResp2.StatusCode);
            Assert.Equal(stringData, await getResp2.Content.AsStringAsync());
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task GetObjectContentRange(S3Provider _, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            await client.PutObjectAsync(bucketName, nameof(GetObjectContentRange), null).ConfigureAwait(false);
            GetObjectResponse getResp = await client.GetObjectAsync(bucketName, nameof(GetObjectContentRange), req => req.Range.Add(0, 2)).ConfigureAwait(false);

            Assert.Equal(206, getResp.StatusCode);
            Assert.Equal(3, getResp.ContentLength);
            Assert.Equal("bytes", getResp.AcceptRanges);
            Assert.Equal("bytes 0-2/4", getResp.ContentRange);
            Assert.Equal("tes", await getResp.Content.AsStringAsync().ConfigureAwait(false));
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task GetObjectLifecycle(S3Provider _, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, nameof(GetObjectLifecycle), null).ConfigureAwait(false);

            //Test lifecycle expiration (yes, we add 2 days. I don't know why Amazon works like this)
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, putResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", putResp.LifeCycleRuleId);

            GetObjectResponse getResp = await client.GetObjectAsync(bucketName, nameof(GetObjectLifecycle)).ConfigureAwait(false);
            Assert.Equal(206, getResp.StatusCode);

            //Test lifecycle expiration
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, getResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", getResp.LifeCycleRuleId);
        }
    }
}