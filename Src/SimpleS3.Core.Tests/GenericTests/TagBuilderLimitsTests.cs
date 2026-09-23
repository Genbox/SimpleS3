using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class TagBuilderLimitsTests
{
    [Theory]
    [InlineData("object", 10)]
    [InlineData("multipart", 10)]
    [InlineData("copy", 10)]
    [InlineData("bucket", 50)]
    public void RequestsApplyTheirResourceTagLimit(string requestType, int maxTags)
    {
        TagBuilder tags = requestType switch
        {
            "object" => new PutObjectRequest("bucket", "key", null).Tags,
            "multipart" => new CreateMultipartUploadRequest("bucket", "key").Tags,
            "copy" => new CopyObjectRequest("source", "key", "bucket", "key").Tags,
            "bucket" => new PutBucketTaggingRequest("bucket", new Dictionary<string, string>()).Tags,
            _ => throw new ArgumentOutOfRangeException(nameof(requestType))
        };

        for (int i = 0; i < maxTags; i++)
            tags.Add($"key{i}", "value");

        Assert.Equal(maxTags, tags.Count());
        Assert.Throws<InvalidOperationException>(() => tags.Add("one-too-many", "value"));
    }

    [Fact]
    public void AllowsUnicodeNamesAndEmptyValues()
    {
        TagBuilder tags = new TagBuilder(10);
        tags.Add("café@𐐀", "東京").Add("empty", "");

        Assert.Equal(2, tags.Count());
        Assert.Throws<ArgumentException>(() => tags.Add("", "value"));
        Assert.Throws<ArgumentException>(() => tags.Add("aws:reserved", "value"));
    }

    [Fact]
    public void StandaloneObjectTaggingRejectsMoreThanTenTags()
    {
        Dictionary<string, string> values = Enumerable.Range(0, 11).ToDictionary(i => $"key{i}", _ => "value", StringComparer.Ordinal);
        PutObjectTaggingRequest request = new PutObjectTaggingRequest("bucket", "key", values);

        Assert.Throws<InvalidOperationException>(() => new PutObjectTaggingRequestMarshal().MarshalRequest(request, new SimpleS3Config()));
    }
}