using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class BucketEncryptionTests
{
    [Fact]
    public void BucketEncryptionCanUnblockSseCustomerKeys()
    {
        S3ServerSideEncryptionRule rule = new S3ServerSideEncryptionRule(SseAlgorithm.Aes256)
        {
            BucketKeyEnabled = false
        };
        rule.BlockedEncryptionTypes.Add(BlockedEncryptionType.None);

        PutBucketEncryptionRequest request = new PutBucketEncryptionRequest("bucket", [rule]);
        PutBucketEncryptionRequestMarshal marshal = new PutBucketEncryptionRequestMarshal();

        using Stream stream = marshal.MarshalRequest(request, new SimpleS3Config("Test", "https://s3.example.com"));
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        string xml = reader.ReadToEnd();
        Assert.Equal(string.Empty, request.QueryParameters["encryption"]);
        Assert.Contains("<ServerSideEncryptionConfiguration xmlns=\"http://s3.amazonaws.com/doc/2006-03-01/\">", xml, StringComparison.Ordinal);
        Assert.Contains("<SSEAlgorithm>AES256</SSEAlgorithm>", xml, StringComparison.Ordinal);
        Assert.Contains("<BlockedEncryptionTypes><EncryptionType>NONE</EncryptionType></BlockedEncryptionTypes>", xml, StringComparison.Ordinal);
        Assert.Contains("<BucketKeyEnabled>false</BucketKeyEnabled>", xml, StringComparison.Ordinal);
    }
}