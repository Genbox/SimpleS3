using System.Security.Cryptography;
using System.Xml.Linq;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class VersioningRequestTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public void VersioningForcesDigestOfExactConfigurationBody(bool? mfaDelete)
    {
        PutBucketVersioningRequest request = new PutBucketVersioningRequest("bucket", true) { MfaDelete = mfaDelete };
        MarshalFactory factory = new MarshalFactory([new PutBucketVersioningRequestMarshal()], []);
        using Stream? body = factory.MarshalRequest(new SimpleS3Config { AlwaysCalculateContentMd5 = false }, request);
        Assert.NotNull(body);
        Assert.Equal(0, body.Position);
        using MemoryStream copy = new MemoryStream();
        body.CopyTo(copy);
        Assert.Equal(Convert.ToBase64String(MD5.HashData(copy.ToArray())), request.Headers["content-md5"]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AuthenticationDoesNotBecomeMfaDeleteSetting(bool enabled)
    {
        PutBucketVersioningRequest request = new PutBucketVersioningRequest("bucket", enabled);
        request.Mfa.SetAuth("device-serial", "123456");
        GenericRequestMapper.Map(request);
        Assert.Equal("device-serial 123456", request.Headers["x-amz-mfa"]);
        using Stream body = new PutBucketVersioningRequestMarshal().MarshalRequest(request, new SimpleS3Config());
        XElement root = XElement.Load(body);
        XNamespace ns = "http://s3.amazonaws.com/doc/2006-03-01/";
        Assert.Equal(ns + "VersioningConfiguration", root.Name);
        Assert.Single(root.Elements());
        Assert.Equal(enabled ? "Enabled" : "Suspended", root.Element(ns + "Status")!.Value);
        Assert.Equal("device-serial 123456", request.Mfa.Build());
        request.Reset();
        GenericRequestMapper.Map(request);
        Assert.False(request.Headers.ContainsKey("x-amz-mfa"));
    }
}