using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ObjectReadMetadataResponseTests
{
    [Fact]
    public void HeadObjectParsesRestoreExpiration()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [AmzHeaders.XAmzRestore] = "ongoing-request=\"false\", expiry-date=\"Sat, 26 Sep 2026 12:00:00 GMT\""
        };
        HeadObjectResponse response = new HeadObjectResponse();

        new HeadObjectResponseMarshal().MarshalResponse(new SimpleS3Config(), response, headers, ContentStream.Null);

        Assert.False(response.RestoreInProgress);
        Assert.Equal(new DateTimeOffset(2026, 9, 26, 12, 0, 0, TimeSpan.Zero), response.RestoreExpiresOn);
    }

    [Fact]
    public void HeadObjectLeavesUnreportedDatesAndPartCountUnset()
    {
        HeadObjectResponse response = new HeadObjectResponse();

        new HeadObjectResponseMarshal().MarshalResponse(new SimpleS3Config(), response, new Dictionary<string, string>(StringComparer.Ordinal), ContentStream.Null);

        Assert.Null(response.LastModified);
        Assert.Null(response.ExpiresOn);
        Assert.Null(response.LockRetainUntil);
        Assert.Null(response.NumberOfParts);
    }

    [Fact]
    public void GetObjectUsesHeadMetadataAndRetainsContent()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [AmzHeaders.XAmzArchiveStatus] = "ARCHIVE_ACCESS",
            [AmzHeaders.XAmzChecksumMd5] = "AQID-2",
            [AmzHeaders.XAmzRequestCharged] = "requester",
            [AmzHeaders.XAmzRestore] = "ongoing-request=\"true\""
        };
        using GetObjectResponse response = new GetObjectResponse();

        new GetObjectResponseMarshal().MarshalResponse(new SimpleS3Config(), response, headers, ContentStream.Null);

        Assert.Equal(ArchiveStatus.ArchiveAccess, response.ArchiveStatus);
        Assert.True(response.RestoreInProgress);
        Assert.Equal(ChecksumAlgorithm.Md5, response.ChecksumAlgorithm);
        Assert.Equal(new byte[] { 1, 2, 3 }, response.Checksum);
        Assert.Equal(ChecksumType.Composite, response.ChecksumType);
        Assert.True(response.RequestCharged);
        Assert.Same(ContentStream.Null, response.Content);
    }
}