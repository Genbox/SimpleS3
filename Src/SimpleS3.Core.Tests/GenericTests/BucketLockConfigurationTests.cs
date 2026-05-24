using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class BucketLockConfigurationTests
{
    [Fact]
    public void BucketLockRetentionUsesTotalDaysRoundedUp()
    {
        PutBucketLockConfigurationRequest request = new PutBucketLockConfigurationRequest("bucket", true)
        {
            LockMode = LockMode.Governance,
            LockRetainUntil = DateTimeOffset.UtcNow.AddDays(2)
        };

        PutBucketLockConfigurationRequestMarshal marshal = new PutBucketLockConfigurationRequestMarshal();

        using Stream stream = marshal.MarshalRequest(request, new SimpleS3Config("Test", "https://s3.example.com"));
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        string xml = reader.ReadToEnd();
        Assert.Contains("<Days>2</Days>", xml, StringComparison.Ordinal);
    }
}