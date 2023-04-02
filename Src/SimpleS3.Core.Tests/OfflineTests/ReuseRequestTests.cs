using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests;

public class ReuseRequestTests : OfflineTestBase
{
    public ReuseRequestTests(ITestOutputHelper outputHelper) : base(outputHelper) {}

    [Fact]
    public async Task ReuseRequestSameData()
    {
        GetObjectRequest req = new GetObjectRequest("testbucket", "testobject");
        req.PartNumber = 5;
        req.VersionId = "versionid";

        string? prevHeader = null;

        for (int i = 0; i < 2; i++)
        {
            GetObjectResponse resp = await ObjectOperations.GetObjectAsync(req).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);

            //None of the essential properties must change
            Assert.Equal("testbucket", req.BucketName);
            Assert.Equal("testobject", req.ObjectKey);

            //None of the user supplied properties must change
            Assert.Equal(5, req.PartNumber);
            Assert.Equal("versionid", req.VersionId);

            await Task.Delay(2000).ConfigureAwait(false);

            //The resolution on signatures is pr. second, so because we wait 2 seconds, it changes, and therefore the signature changes too
            string currentHeader = req.Headers[HttpHeaders.Authorization];

            if (i > 0)
                Assert.NotEqual(prevHeader, currentHeader);

            prevHeader = currentHeader;
        }
    }

    [Fact]
    public async Task ReuseRequestDifferentData()
    {
        GetObjectRequest req = new GetObjectRequest("testbucket", "notused-setbelow");
        req.PartNumber = 5;
        req.VersionId = "versionid";

        string? prevHeader = null;

        for (int i = 0; i < 10; i++)
        {
            string key = i.ToString();
            req.ObjectKey = key;

            GetObjectResponse resp = await ObjectOperations.GetObjectAsync(req).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);

            //The key must not change after the request is sent
            Assert.Equal(key, req.ObjectKey);

            //None of the user supplied properties must change
            Assert.Equal(5, req.PartNumber);
            Assert.Equal("versionid", req.VersionId);

            //The authorization header should change with each request due changes in ObjectKey
            string currentHeader = req.Headers[HttpHeaders.Authorization];

            if (i > 0)
                Assert.NotEqual(prevHeader, currentHeader);

            prevHeader = currentHeader;
        }
    }
}