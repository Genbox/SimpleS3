using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class BucketLifecycleConfigurationTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void VersionExpirationAndMultipartAbortRemainIndependent(bool includeAbort, bool abortFirst)
    {
        const string expiration = "<NoncurrentVersionExpiration><NoncurrentDays>30</NoncurrentDays></NoncurrentVersionExpiration>";
        string abort = includeAbort ? "<AbortIncompleteMultipartUpload><DaysAfterInitiation>7</DaysAfterInitiation></AbortIncompleteMultipartUpload>" : string.Empty;
        string actions = abortFirst ? abort + expiration : expiration + abort;
        byte[] data = Encoding.UTF8.GetBytes("<LifecycleConfiguration><Rule><ID>cleanup</ID><Status>Enabled</Status>" + actions + "</Rule></LifecycleConfiguration>");
        using MemoryStream input = new MemoryStream(data);
        using ContentStream content = new ContentStream(input, data.Length);
        GetBucketLifecycleConfigurationResponse response = new GetBucketLifecycleConfigurationResponse();

        new GetBucketLifecycleConfigurationResponseMarshal().MarshalResponse(new SimpleS3Config(), response, new Dictionary<string, string>(StringComparer.Ordinal), content);

        S3Rule rule = Assert.Single(response.Rules);
        Assert.Equal(30, rule.NonCurrentVersionExpirationDays);
        Assert.Equal(includeAbort ? 7 : null, rule.AbortIncompleteMultipartUploadDays);
    }
}