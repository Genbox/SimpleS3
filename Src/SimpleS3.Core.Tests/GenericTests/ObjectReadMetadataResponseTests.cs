using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
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
}