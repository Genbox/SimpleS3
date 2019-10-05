using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class ServerSideEncryptionTests : LiveTestBase
    {
        public ServerSideEncryptionTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Theory]
        [InlineData(SseAlgorithm.Aes256)]
        [InlineData(SseAlgorithm.AwsKms)]
        public async Task ServerSideEncryption(SseAlgorithm algorithm)
        {
            PutObjectResponse resp = await UploadAsync(nameof(ServerSideEncryption), request => request.SseAlgorithm = algorithm).ConfigureAwait(false);
            Assert.Equal(algorithm, resp.SseAlgorithm);

            await AssertAsync(nameof(ServerSideEncryption)).ConfigureAwait(false);
        }

        [Fact]
        public async Task ServerSideEncryptionCustomerKey()
        {
            byte[] key = new byte[32];
            new Random(42).NextBytes(key);

            byte[] keyHash = CryptoHelper.Md5Hash(key);

            PutObjectResponse pResp = await UploadAsync(nameof(ServerSideEncryptionCustomerKey), request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyHash;
            }).ConfigureAwait(false);

            Assert.Equal(SseCustomerAlgorithm.Aes256, pResp.SseCustomerAlgorithm);
            Assert.Equal(keyHash, pResp.SseCustomerKeyMd5);

            await AssertAsync(nameof(ServerSideEncryptionCustomerKey), request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyHash;
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ServerSideEncryptionCustomerKeyFluid()
        {
            byte[] key = new byte[32];
            new Random(42).NextBytes(key);

            PutObjectResponse pResp = await UploadTransferAsync(nameof(ServerSideEncryptionCustomerKey), upload => upload.WithEncryptionCustomerKey(key)).ConfigureAwait(false);

            Assert.Equal(SseCustomerAlgorithm.Aes256, pResp.SseCustomerAlgorithm);

            //TODO: Missing support for Transfer Download
            //await AssertAsync(nameof(ServerSideEncryptionCustomerKeyFluid), request =>
            //{
            //    request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            //    request.SseCustomerKey = key;
            //    request.SseCustomerKeyMd5 = keyHash;
            //}).ConfigureAwait(false);
        }

        [Fact]
        public async Task ServerSideEncryptionFluid()
        {
            PutObjectResponse resp = await UploadTransferAsync(nameof(ServerSideEncryptionFluid) + "aes", upload => upload.WithEncryption()).ConfigureAwait(false);
            Assert.Equal(SseAlgorithm.Aes256, resp.SseAlgorithm);

            await AssertAsync(nameof(ServerSideEncryptionFluid) + "aes").ConfigureAwait(false);

            resp = await UploadTransferAsync(nameof(ServerSideEncryptionFluid) + "kms", upload => upload.WithEncryptionKms()).ConfigureAwait(false);
            Assert.Equal(SseAlgorithm.AwsKms, resp.SseAlgorithm);

            await AssertAsync(nameof(ServerSideEncryptionFluid) + "kms").ConfigureAwait(false);
        }
    }
}