using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class BucketLoggingResponseTests
{
    [Fact]
    public void MissingPrefixAndAclGrantArePreserved()
    {
        GetBucketLoggingResponse response = Parse("""
                                                  <BucketLoggingStatus xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
                                                    <LoggingEnabled>
                                                      <TargetBucket>logs</TargetBucket>
                                                      <TargetGrants>
                                                        <Grant>
                                                          <Grantee xsi:type="CanonicalUser"><ID>owner</ID></Grantee>
                                                          <Permission>READ_ACP</Permission>
                                                        </Grant>
                                                      </TargetGrants>
                                                    </LoggingEnabled>
                                                  </BucketLoggingStatus>
                                                  """);

        Assert.Equal("logs", response.LoggingConfiguration?.TargetBucket);
        Assert.Null(response.LoggingConfiguration?.TargetPrefix);
        Assert.Equal(S3Permission.ReadAcp, Assert.Single(response.LoggingConfiguration!.TargetGrants).Permission);
    }

    [Theory]
    [InlineData("DeliveryTime", S3LoggingObjectKeyFormat.PartitionedPrefixDeliveryTime)]
    [InlineData("UnknownDateSource", null)]
    public void PartitionDateSourceIsNotSilentlyTreatedAsEventTime(string source, S3LoggingObjectKeyFormat? expected)
    {
        GetBucketLoggingResponse response = Parse($"<BucketLoggingStatus><LoggingEnabled><TargetBucket>logs</TargetBucket><TargetObjectKeyFormat><PartitionedPrefix><PartitionDateSource>{source}</PartitionDateSource></PartitionedPrefix></TargetObjectKeyFormat></LoggingEnabled></BucketLoggingStatus>");

        Assert.Equal(expected, response.LoggingConfiguration?.TargetObjectKeyFormat);
    }

    private static GetBucketLoggingResponse Parse(string xml)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(xml);
        using MemoryStream body = new MemoryStream(bytes);
        using ContentStream content = new ContentStream(body, body.Length);
        GetBucketLoggingResponse response = new GetBucketLoggingResponse();
        new GetBucketLoggingResponseMarshal().MarshalResponse(new SimpleS3Config(), response, new Dictionary<string, string>(), content);
        return response;
    }
}