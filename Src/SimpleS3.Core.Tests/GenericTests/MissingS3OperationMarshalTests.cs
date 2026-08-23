using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
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
    public void PutBucketWebsiteWritesWebsiteXml()
    {
        S3WebsiteConfiguration website = new S3WebsiteConfiguration
        {
            IndexDocumentSuffix = "index.html",
            ErrorDocumentKey = "error.html"
        };
        website.RoutingRules.Add(new S3RoutingRule(new S3Redirect { ReplaceKeyWith = "docs/index.html", HttpRedirectCode = "302" }) { Condition = new S3RoutingRuleCondition { KeyPrefixEquals = "docs/" } });
        PutBucketWebsiteRequest request = new PutBucketWebsiteRequest("bucket", website);

        using Stream stream = Assert.IsAssignableFrom<Stream>(new PutBucketWebsiteRequestMarshal().MarshalRequest(request, CreateConfig()));
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        string xml = reader.ReadToEnd();
        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.Website]);
        Assert.Contains("<Suffix>index.html</Suffix>", xml, StringComparison.Ordinal);
        Assert.Contains("<Key>error.html</Key>", xml, StringComparison.Ordinal);
        Assert.Contains("<KeyPrefixEquals>docs/</KeyPrefixEquals>", xml, StringComparison.Ordinal);
        Assert.Contains("<ReplaceKeyWith>docs/index.html</ReplaceKeyWith>", xml, StringComparison.Ordinal);
    }

    [Fact]
    public void GetBucketWebsiteParsesWebsiteConfiguration()
    {
        const string xml = """
                           <WebsiteConfiguration><IndexDocument><Suffix>index.html</Suffix></IndexDocument><ErrorDocument><Key>error.html</Key></ErrorDocument><RoutingRules><RoutingRule><Condition><HttpErrorCodeReturnedEquals>404</HttpErrorCodeReturnedEquals></Condition><Redirect><HostName>example.com</HostName><Protocol>https</Protocol><ReplaceKeyPrefixWith>errors/</ReplaceKeyPrefixWith></Redirect></RoutingRule></RoutingRules></WebsiteConfiguration>
                           """;
        GetBucketWebsiteResponse response = new GetBucketWebsiteResponse();

        using ContentStream content = CreateContent(xml);
        new GetBucketWebsiteResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.Equal("index.html", response.WebsiteConfiguration.IndexDocumentSuffix);
        Assert.Equal("error.html", response.WebsiteConfiguration.ErrorDocumentKey);
        S3RoutingRule rule = Assert.Single(response.WebsiteConfiguration.RoutingRules);
        Assert.Equal("404", rule.Condition?.HttpErrorCodeReturnedEquals);
        Assert.Equal("example.com", rule.Redirect.HostName);
        Assert.Equal("https", rule.Redirect.Protocol);
        Assert.Equal("errors/", rule.Redirect.ReplaceKeyPrefixWith);
    }

    [Fact]
    public void BucketConfigurationDeleteRequestsSetSubresourceAndId()
    {
        DeleteBucketAnalyticsConfigurationRequest analytics = new DeleteBucketAnalyticsConfigurationRequest("bucket", "config-id");
        DeleteBucketReplicationRequest replication = new DeleteBucketReplicationRequest("bucket");

        new DeleteBucketAnalyticsConfigurationRequestMarshal().MarshalRequest(analytics, CreateConfig());
        new DeleteBucketReplicationRequestMarshal().MarshalRequest(replication, CreateConfig());

        Assert.Equal(string.Empty, analytics.QueryParameters[AmzParameters.Analytics]);
        Assert.Equal("config-id", analytics.QueryParameters[AmzParameters.Id]);
        Assert.Equal(string.Empty, replication.QueryParameters[AmzParameters.Replication]);
    }

    [Fact]
    public void GetPublicAccessBlockParsesAllCompactElements()
    {
        const string xml = "<PublicAccessBlockConfiguration><BlockPublicAcls>true</BlockPublicAcls><IgnorePublicAcls>false</IgnorePublicAcls><BlockPublicPolicy>true</BlockPublicPolicy><RestrictPublicBuckets>false</RestrictPublicBuckets></PublicAccessBlockConfiguration>";
        GetPublicAccessBlockResponse response = new GetPublicAccessBlockResponse();

        using ContentStream content = CreateContent(xml);
        new GetPublicAccessBlockResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.True(response.BlockPublicAcls);
        Assert.False(response.IgnorePublicAcls);
        Assert.True(response.BlockPublicPolicy);
        Assert.False(response.RestrictPublicBuckets);
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
    public void GetObjectRetentionParsesAllCompactElements()
    {
        const string xml = "<Retention><Mode>GOVERNANCE</Mode><RetainUntilDate>2026-05-25T12:30:00.000Z</RetainUntilDate></Retention>";
        GetObjectRetentionResponse response = new GetObjectRetentionResponse();

        using ContentStream content = CreateContent(xml);
        new GetObjectRetentionResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.Equal(LockMode.Governance, response.LockMode);
        Assert.Equal(new DateTimeOffset(2026, 5, 25, 12, 30, 0, TimeSpan.Zero), response.LockRetainUntil);
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

    [Fact]
    public void GetObjectAttributesWritesAttributesHeaderAndQuery()
    {
        GetObjectAttributesRequest request = new GetObjectAttributesRequest("bucket", "object", [ObjectAttribute.ETag, ObjectAttribute.Checksum, ObjectAttribute.ObjectParts])
        {
            MaxParts = 10,
            PartNumberMarker = 2
        };

        new GetObjectAttributesRequestMarshal().MarshalRequest(request, CreateConfig());

        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.Attributes]);
        Assert.Equal("ETag,Checksum,ObjectParts", request.Headers[AmzHeaders.XAmzObjectAttributes]);
        Assert.Equal("10", request.QueryParameters[AmzParameters.MaxParts]);
        Assert.Equal("2", request.QueryParameters[AmzParameters.PartNumberMarker]);
    }

    [Fact]
    public void GetObjectAttributesParsesAttributesAndParts()
    {
        const string xml = """
                           <GetObjectAttributesResponse><ETag>&quot;etag&quot;</ETag><Checksum><ChecksumCRC32>AAAAAA==</ChecksumCRC32><ChecksumType>COMPOSITE</ChecksumType></Checksum><ObjectSize>123</ObjectSize><StorageClass>STANDARD</StorageClass><ObjectParts><TotalPartsCount>2</TotalPartsCount><PartNumberMarker>1</PartNumberMarker><NextPartNumberMarker>2</NextPartNumberMarker><MaxParts>10</MaxParts><IsTruncated>true</IsTruncated><Part><PartNumber>2</PartNumber><Size>100</Size><ChecksumSHA256>AQIDBA==</ChecksumSHA256></Part></ObjectParts></GetObjectAttributesResponse>
                           """;
        GetObjectAttributesResponse response = new GetObjectAttributesResponse();

        using ContentStream content = CreateContent(xml);
        new GetObjectAttributesResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string> { [AmzHeaders.XAmzVersionId] = "version" }, content);

        Assert.Equal("\"etag\"", response.ETag);
        Assert.Equal("version", response.VersionId);
        Assert.Equal(123, response.ObjectSize);
        Assert.Equal(StorageClass.Standard, response.StorageClass);
        Assert.Equal(ChecksumType.Composite, response.ChecksumType);
        Assert.Equal([0, 0, 0, 0], response.ChecksumCrc32);
        Assert.True(response.IsTruncated);
        Assert.Equal(2, response.TotalPartsCount);
        S3ObjectAttributePart part = Assert.Single(response.Parts);
        Assert.Equal(2, part.PartNumber);
        Assert.Equal(100, part.Size);
        Assert.Equal([1, 2, 3, 4], part.ChecksumSha256);
    }

    [Fact]
    public void ObjectAnnotationRequestsWriteAnnotationParameters()
    {
        GetObjectAnnotationRequest get = new GetObjectAnnotationRequest("bucket", "object", "review") { EnableChecksum = true };
        DeleteObjectAnnotationRequest delete = new DeleteObjectAnnotationRequest("bucket", "object", "review")
        {
            ObjectIfMatch = "etag",
            BypassGovernanceRetention = true
        };
        ListObjectAnnotationsRequest list = new ListObjectAnnotationsRequest("bucket", "object")
        {
            AnnotationPrefix = "rev",
            ContinuationToken = "next",
            MaxAnnotationResults = 20
        };

        new GetObjectAnnotationRequestMarshal().MarshalRequest(get, CreateConfig());
        GenericRequestMapper.Map(delete);
        new DeleteObjectAnnotationRequestMarshal().MarshalRequest(delete, CreateConfig());
        new ListObjectAnnotationsRequestMarshal().MarshalRequest(list, CreateConfig());

        Assert.Equal(string.Empty, get.QueryParameters[AmzParameters.Annotation]);
        Assert.Equal("review", get.QueryParameters[AmzParameters.AnnotationName]);
        Assert.Equal("ENABLED", get.Headers[AmzHeaders.XAmzChecksumMode]);
        Assert.Equal("etag", delete.Headers[AmzHeaders.XAmzObjectIfMatch]);
        Assert.Equal("true", delete.Headers[AmzHeaders.XAmzBypassGovernanceRetention]);
        Assert.Equal("rev", list.QueryParameters[AmzParameters.AnnotationPrefix]);
        Assert.Equal("next", list.QueryParameters[AmzParameters.ContinuationToken]);
        Assert.Equal("20", list.QueryParameters[AmzParameters.MaxAnnotationResults]);
    }

    [Fact]
    public void PutObjectAnnotationRetainsRawContent()
    {
        using MemoryStream content = new MemoryStream(Encoding.UTF8.GetBytes("annotation body"));
        PutObjectAnnotationRequest request = new PutObjectAnnotationRequest("bucket", "object", "review", content) { ObjectIfMatch = "etag" };

        Stream? marshalledContent = new PutObjectAnnotationRequestMarshal().MarshalRequest(request, CreateConfig());

        Assert.Null(marshalledContent);
        Assert.Same(content, request.Content);
        Assert.IsAssignableFrom<ISupportStreaming>(request);
        Assert.IsNotAssignableFrom<IContentMd5Config>(request);
        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.Annotation]);
        Assert.Equal("review", request.QueryParameters[AmzParameters.AnnotationName]);
        Assert.Equal("etag", request.Headers[AmzHeaders.XAmzObjectIfMatch]);
    }

    [Fact]
    public void GetObjectAnnotationMapsContentAndMetadata()
    {
        GetObjectAnnotationResponse response = new GetObjectAnnotationResponse();
        Dictionary<string, string> headers = new Dictionary<string, string>
        {
            [HttpHeaders.ETag] = "etag",
            [HttpHeaders.LastModified] = "Sun, 23 Aug 2026 12:00:00 GMT",
            [AmzHeaders.XAmzVersionId] = "version",
            [AmzHeaders.XAmzSse] = "aws:kms",
            [AmzHeaders.XAmzSseAwsKmsKeyId] = "key-id",
            [AmzHeaders.XAmzChecksumSha256] = "AQIDBA==",
            [AmzHeaders.XAmzChecksumType] = "FULL_OBJECT",
            [AmzHeaders.XAmzReplicationStatus] = "COMPLETE"
        };

        using ContentStream content = CreateContent("annotation body");
        new GetObjectAnnotationResponseMarshal().MarshalResponse(CreateConfig(), response, headers, content);

        Assert.Same(content, response.Content);
        Assert.Equal("etag", response.ETag);
        Assert.Equal("version", response.VersionId);
        Assert.Equal(SseAlgorithm.AwsKms, response.SseAlgorithm);
        Assert.Equal("key-id", response.SseKmsKeyId);
        Assert.Equal(ChecksumAlgorithm.Sha256, response.ChecksumAlgorithm);
        Assert.Equal(ChecksumType.FullObject, response.ChecksumType);
        Assert.Equal([1, 2, 3, 4], response.Checksum);
        Assert.Equal(ReplicationStatus.Complete, response.ReplicationStatus);
    }

    [Fact]
    public void ListObjectAnnotationsParsesEntriesAndPagination()
    {
        const string xml = """
                           <ListObjectAnnotationsOutput><Annotations><AnnotationEntry><AnnotationName>review</AnnotationName><LastModified>2026-08-23T12:00:00.000Z</LastModified><Size>42</Size><ETag>etag</ETag><ChecksumAlgorithm>CRC64NVME</ChecksumAlgorithm><ReplicationStatus>COMPLETE</ReplicationStatus></AnnotationEntry></Annotations><Bucket>bucket</Bucket><Key>object</Key><AnnotationPrefix>rev</AnnotationPrefix><MaxAnnotationResults>20</MaxAnnotationResults><AnnotationCount>1</AnnotationCount><ContinuationToken>current</ContinuationToken><NextContinuationToken>next</NextContinuationToken></ListObjectAnnotationsOutput>
                           """;
        ListObjectAnnotationsResponse response = new ListObjectAnnotationsResponse();

        using ContentStream content = CreateContent(xml);
        new ListObjectAnnotationsResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        S3Annotation annotation = Assert.Single(response.Annotations);
        Assert.Equal("review", annotation.Name);
        Assert.Equal(42, annotation.Size);
        Assert.Equal("etag", annotation.ETag);
        Assert.Equal(ChecksumAlgorithm.Crc64Nvme, Assert.Single(annotation.ChecksumAlgorithms));
        Assert.Equal(ReplicationStatus.Complete, annotation.ReplicationStatus);
        Assert.Equal("bucket", response.BucketName);
        Assert.Equal("object", response.ObjectKey);
        Assert.Equal(1, response.AnnotationCount);
        Assert.Equal("next", response.NextContinuationToken);
    }

    [Fact]
    public void UpdateMetadataAnnotationTableWritesConfigurationXml()
    {
        S3AnnotationTableConfiguration configuration = new S3AnnotationTableConfiguration(AnnotationTableConfigurationState.Enabled)
        {
            Role = "arn:aws:iam::123456789012:role/metadata",
            EncryptionConfiguration = new S3MetadataTableEncryptionConfiguration(SseAlgorithm.AwsKms)
            {
                KmsKeyArn = "arn:aws:kms:us-east-1:123456789012:key/key-id"
            }
        };
        UpdateBucketMetadataAnnotationTableConfigurationRequest request = new UpdateBucketMetadataAnnotationTableConfigurationRequest("bucket", configuration);
        request.ChecksumAlgorithm = ChecksumAlgorithm.Sha256;
        request.Checksum = [1, 2, 3, 4];

        GenericRequestMapper.Map(request);
        using Stream stream = new UpdateBucketMetadataAnnotationTableConfigurationRequestMarshal().MarshalRequest(request, CreateConfig());
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        string xml = reader.ReadToEnd();

        Assert.Equal(string.Empty, request.QueryParameters[AmzParameters.MetadataAnnotationTable]);
        Assert.Equal("AQIDBA==", request.Headers[AmzHeaders.XAmzChecksumSha256]);
        Assert.Contains("<ConfigurationState>ENABLED</ConfigurationState>", xml, StringComparison.Ordinal);
        Assert.Contains("<SseAlgorithm>aws:kms</SseAlgorithm>", xml, StringComparison.Ordinal);
        Assert.Contains("<KmsKeyArn>arn:aws:kms:us-east-1:123456789012:key/key-id</KmsKeyArn>", xml, StringComparison.Ordinal);
        Assert.Contains("<Role>arn:aws:iam::123456789012:role/metadata</Role>", xml, StringComparison.Ordinal);
    }

    [Fact]
    public void PutBucketLifecycleWritesExpirationAtMidnightUtc()
    {
        S3Rule rule = new S3Rule("rule", true)
        {
            Expiration = new S3Expiration(new DateTimeOffset(2026, 8, 24, 15, 30, 0, TimeSpan.FromHours(2)))
        };
        PutBucketLifecycleConfigurationRequest request = new PutBucketLifecycleConfigurationRequest("bucket", [rule]);

        using Stream stream = new PutBucketLifecycleConfigurationRequestMarshal().MarshalRequest(request, CreateConfig());
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        Assert.Contains("<Date>2026-08-24T00:00:00Z</Date>", reader.ReadToEnd(), StringComparison.Ordinal);
    }

    [Fact]
    public void CompleteMultipartUploadParsesSuccessfulResultRoot()
    {
        const string xml = "<CompleteMultipartUploadResult><Location>https://example.com/object</Location><Bucket>bucket</Bucket><Key>key</Key><ETag>&quot;etag&quot;</ETag><ChecksumType>COMPOSITE</ChecksumType><ChecksumSHA1>AQIDBA==-1</ChecksumSHA1></CompleteMultipartUploadResult>";
        CompleteMultipartUploadResponse response = new CompleteMultipartUploadResponse();

        using ContentStream content = CreateContent(xml);
        new CompleteMultipartUploadResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.Equal("https://example.com/object", response.Location);
        Assert.Equal("bucket", response.BucketName);
        Assert.Equal("key", response.ObjectKey);
        Assert.Equal("\"etag\"", response.ETag);
        Assert.Equal(ChecksumType.Composite, response.ChecksumType);
        Assert.Equal(ChecksumAlgorithm.Sha1, response.ChecksumAlgorithm);
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, response.Checksum);
    }

    private static SimpleS3Config CreateConfig() => new SimpleS3Config("Test", "https://s3.example.com");

    private static ContentStream CreateContent(string xml)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return new ContentStream(stream, stream.Length);
    }
}