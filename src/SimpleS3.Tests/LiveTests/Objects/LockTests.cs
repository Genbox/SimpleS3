using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class LockTests : LiveTestBase
    {
        public LockTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Theory]
        [InlineData(Core.Enums.LockMode.Compliance)]
        [InlineData(Core.Enums.LockMode.Governance)]
        public async Task LockMode(LockMode lockMode)
        {
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string objectKey = $"{nameof(LockMode)}-{lockMode}-{Guid.NewGuid()}";

            await UploadAsync(objectKey, request =>
            {
                request.LockMode = lockMode;
                request.LockRetainUntil = lockRetainUntil;
            }).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(objectKey).ConfigureAwait(false);
            Assert.Equal(lockMode, resp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, resp.LockRetainUntilDate.DateTime, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(Core.Enums.LockMode.Compliance)]
        [InlineData(Core.Enums.LockMode.Governance)]
        public async Task LockModeFluent(LockMode lockMode)
        {
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string objectKey = $"{nameof(LockModeFluent)}-{lockMode}-{Guid.NewGuid()}";

            await UploadTransferAsync(objectKey, upload => upload.WithLock(lockMode, lockRetainUntil)).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(objectKey).ConfigureAwait(false);
            Assert.Equal(lockMode, resp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, resp.LockRetainUntilDate.DateTime, TimeSpan.FromSeconds(1));
        }
    }
}