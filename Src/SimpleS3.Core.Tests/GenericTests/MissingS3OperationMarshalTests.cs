using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class MissingS3OperationMarshalTests
{
    [Fact]
    public void PutBucketAclSetsAclQuery()
    {
        PutBucketAclRequest request = new PutBucketAclRequest("bucket");

        new PutBucketAclRequestMarshal().MarshalRequest(request, CreateConfig());

        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.Acl]);
    }

    [Fact]
    public void GetBucketAclParsesGrants()
    {
        const string xml = """
                           <AccessControlPolicy xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><Owner><ID>owner-id</ID><DisplayName>owner</DisplayName></Owner><AccessControlList><Grant><Grantee xsi:type="CanonicalUser"><ID>grantee-id</ID><DisplayName>grantee</DisplayName></Grantee><Permission>FULL_CONTROL</Permission></Grant></AccessControlList></AccessControlPolicy>
                           """;
        GetBucketAclResponse response = new GetBucketAclResponse();

        using ContentStream content = CreateContent(xml);
        new GetBucketAclResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.Equal("owner-id", response.Owner.Id);
        Assert.Equal("owner", response.Owner.Name);
        S3Grant grant = Assert.Single(response.Grants);
        Assert.Equal(GrantType.CanonicalUser, grant.Grantee.Type);
        Assert.Equal("grantee-id", grant.Grantee.Id);
        Assert.Equal(S3Permission.FullControl, grant.Permission);
    }

    [Fact]
    public void PutBucketCorsWritesCorsXml()
    {
        S3CorsRule rule = new S3CorsRule { Id = "rule", MaxAgeSeconds = 300 };
        rule.AllowedOrigins.Add("https://example.com");
        rule.AllowedMethods.Add("GET");
        rule.AllowedHeaders.Add("*");
        rule.ExposeHeaders.Add("ETag");
        PutBucketCorsRequest request = new PutBucketCorsRequest("bucket", [rule]);

        using Stream stream = new PutBucketCorsRequestMarshal().MarshalRequest(request, CreateConfig());
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        string xml = reader.ReadToEnd();
        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.Cors]);
        Assert.Contains("<AllowedOrigin>https://example.com</AllowedOrigin>", xml, StringComparison.Ordinal);
        Assert.Contains("<AllowedMethod>GET</AllowedMethod>", xml, StringComparison.Ordinal);
        Assert.Contains("<ExposeHeader>ETag</ExposeHeader>", xml, StringComparison.Ordinal);
    }

    [Fact]
    public void GetBucketCorsParsesRules()
    {
        const string xml = """
                           <CORSConfiguration><CORSRule><ID>rule</ID><AllowedOrigin>*</AllowedOrigin><AllowedMethod>GET</AllowedMethod><AllowedHeader>*</AllowedHeader><ExposeHeader>ETag</ExposeHeader><MaxAgeSeconds>300</MaxAgeSeconds></CORSRule></CORSConfiguration>
                           """;
        GetBucketCorsResponse response = new GetBucketCorsResponse();

        using ContentStream content = CreateContent(xml);
        new GetBucketCorsResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        S3CorsRule rule = Assert.Single(response.Rules);
        Assert.Equal("rule", rule.Id);
        Assert.Equal("*", Assert.Single(rule.AllowedOrigins));
        Assert.Equal("GET", Assert.Single(rule.AllowedMethods));
        Assert.Equal("*", Assert.Single(rule.AllowedHeaders));
        Assert.Equal("ETag", Assert.Single(rule.ExposeHeaders));
        Assert.Equal(300, rule.MaxAgeSeconds);
    }

    [Fact]
    public void PutBucketOwnershipControlsWritesOwnershipXml()
    {
        PutBucketOwnershipControlsRequest request = new PutBucketOwnershipControlsRequest("bucket", [new S3OwnershipControlsRule(ObjectOwnership.BucketOwnerEnforced)]);

        using Stream stream = new PutBucketOwnershipControlsRequestMarshal().MarshalRequest(request, CreateConfig());
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        string xml = reader.ReadToEnd();
        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.OwnershipControls]);
        Assert.Contains("<ObjectOwnership>BucketOwnerEnforced</ObjectOwnership>", xml, StringComparison.Ordinal);
    }

    [Fact]
    public void GetBucketRequestPaymentParsesPayer()
    {
        const string xml = "<RequestPaymentConfiguration><Payer>Requester</Payer></RequestPaymentConfiguration>";
        GetBucketRequestPaymentResponse response = new GetBucketRequestPaymentResponse();

        using ContentStream content = CreateContent(xml);
        new GetBucketRequestPaymentResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.Equal(RequestPaymentPayer.Requester, response.Payer);
    }

    [Fact]
    public void PutObjectTaggingWritesTaggingXml()
    {
        PutObjectTaggingRequest request = new PutObjectTaggingRequest("bucket", "object", new Dictionary<string, string> { ["Key"] = "Value" });

        using Stream stream = new PutObjectTaggingRequestMarshal().MarshalRequest(request, CreateConfig());
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        string xml = reader.ReadToEnd();
        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.Tagging]);
        Assert.Contains("<Tagging xmlns=\"http://s3.amazonaws.com/doc/2006-03-01/\">", xml, StringComparison.Ordinal);
        Assert.Contains("<Key>Key</Key>", xml, StringComparison.Ordinal);
        Assert.Contains("<Value>Value</Value>", xml, StringComparison.Ordinal);
    }

    [Fact]
    public void PutObjectRetentionWritesRetentionXml()
    {
        DateTimeOffset retainUntil = new DateTimeOffset(2026, 5, 25, 12, 30, 0, TimeSpan.Zero);
        PutObjectRetentionRequest request = new PutObjectRetentionRequest("bucket", "object", LockMode.Governance, retainUntil);

        using Stream stream = new PutObjectRetentionRequestMarshal().MarshalRequest(request, CreateConfig());
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        string xml = reader.ReadToEnd();
        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.Retention]);
        Assert.Contains("<Mode>GOVERNANCE</Mode>", xml, StringComparison.Ordinal);
        Assert.Contains("<RetainUntilDate>" + ValueHelper.DateToString(retainUntil, DateTimeFormat.Iso8601DateTimeExt) + "</RetainUntilDate>", xml, StringComparison.Ordinal);
    }

    [Fact]
    public void UploadPartCopyWritesCopyHeaders()
    {
        UploadPartCopyRequest request = new UploadPartCopyRequest("source-bucket", "path/object.txt", "dest-bucket", "copy.txt", "upload-id", 3)
        {
            CopySourceRange = "bytes=0-10"
        };

        new UploadPartCopyRequestMarshal().MarshalRequest(request, CreateConfig());

        Assert.Equal("/source-bucket/path/object.txt", request.Headers[AmzHeaders.XAmzCopySource]);
        Assert.Equal("bytes=0-10", request.Headers[AmzHeaders.XAmzCopySourceRange]);
    }

    [Fact]
    public void GetBucketEncryptionParsesRules()
    {
        const string xml = """
                           <ServerSideEncryptionConfiguration>
                             <Rule>
                               <ApplyServerSideEncryptionByDefault>
                                 <SSEAlgorithm>aws:kms</SSEAlgorithm>
                                 <KMSMasterKeyID>key-id</KMSMasterKeyID>
                               </ApplyServerSideEncryptionByDefault>
                               <BucketKeyEnabled>true</BucketKeyEnabled>
                               <BlockedEncryptionTypes><EncryptionType>SSE-C</EncryptionType></BlockedEncryptionTypes>
                             </Rule>
                           </ServerSideEncryptionConfiguration>
                           """;
        GetBucketEncryptionResponse response = new GetBucketEncryptionResponse();

        using ContentStream content = CreateContent(xml);
        new GetBucketEncryptionResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        var rule = Assert.Single(response.Rules);
        Assert.Equal(SseAlgorithm.AwsKms, rule.SseAlgorithm);
        Assert.Equal("key-id", rule.KmsMasterKeyId);
        Assert.True(rule.BucketKeyEnabled);
        Assert.Equal(BlockedEncryptionType.SseC, Assert.Single(rule.BlockedEncryptionTypes));
    }

    [Fact]
    public void GetObjectTaggingParsesTags()
    {
        const string xml = """
                           <Tagging><TagSet><Tag><Key>Key</Key><Value>Value</Value></Tag></TagSet></Tagging>
                           """;
        GetObjectTaggingResponse response = new GetObjectTaggingResponse();

        using ContentStream content = CreateContent(xml);
        new GetObjectTaggingResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.Equal("Value", response.Tags["Key"]);
    }

    private static SimpleS3Config CreateConfig() => new SimpleS3Config("Test", "https://s3.example.com");

    private static ContentStream CreateContent(string xml)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return new ContentStream(stream, stream.Length);
    }
}