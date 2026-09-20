using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Internals.Fluent;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class FluentDownloadTests
{
    [Theory]
    [InlineData("*")]
    [InlineData("W/\"revision\"")]
    [InlineData("\"first\", \"second\"")]
    [InlineData("\"comma,inside\"")]
    public void CopiedEtagConditionsPreserveBuilderRepresentation(string condition)
    {
        ETagBuilder source = new ETagBuilder();
        source.Set(condition);
        string expected = source.Build()!;
        Download download = new Download(null!, null!, "bucket", "key");
        download.WithEtagConditional(source, source);
        GetObjectRequest target = new GetObjectRequest("bucket", "key");
        download.CopyProperties(target);
        Assert.Equal(expected, target.IfETagMatch.Build());
        Assert.Equal(expected, target.IfETagNotMatch.Build());
        Assert.NotSame(source, target.IfETagMatch);
        Assert.NotSame(source, target.IfETagNotMatch);
        target.IfETagMatch.Set("\"changed\"");
        Assert.Equal(expected, target.IfETagNotMatch.Build());
        Assert.Equal(expected, source.Build());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void RequestEtagChangesDoNotMutateFluentConditions(int configuration)
    {
        Download download = new Download(null!, null!, "bucket", "key");
        ETagBuilder match = new ETagBuilder();
        ETagBuilder nonmatch = new ETagBuilder();
        match.Set("\"original\"");
        nonmatch.Set("\"different\"");
        if (configuration == 1)
            download.WithEtagConditional("\"original\"", "\"different\"");
        else if (configuration == 2)
            download.WithEtagConditional(match, nonmatch);
        GetObjectRequest first = new GetObjectRequest("bucket", "key");
        download.CopyProperties(first);
        string? originalMatch = first.IfETagMatch.Build();
        string? originalNonmatch = first.IfETagNotMatch.Build();
        first.IfETagMatch.Set("\"revision\"");
        first.IfETagNotMatch.Set("\"changed\"");
        GetObjectRequest second = new GetObjectRequest("bucket", "key");
        download.CopyProperties(second);
        Assert.Equal(originalMatch, second.IfETagMatch.Build());
        Assert.Equal(originalNonmatch, second.IfETagNotMatch.Build());
        Assert.Equal("\"original\"", match.Build());
        Assert.Equal("\"different\"", nonmatch.Build());

        if (configuration != 0)
        {
            Assert.Equal(match.Build(), originalMatch);
            Assert.Equal(nonmatch.Build(), originalNonmatch);
        }
    }

    [Fact]
    public void HelperGeneratedRangeDoesNotPolluteNextRequest()
    {
        Download download = new Download(null!, null!, "bucket", "key");
        GetObjectRequest first = new GetObjectRequest("bucket", "key");
        download.CopyProperties(first);
        first.Range.Add(0, 99);
        GetObjectRequest second = new GetObjectRequest("bucket", "key");
        download.CopyProperties(second);
        Assert.False(second.Range.HasData());
        Assert.True(first.Range.HasData());
    }
}