using System.Text;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Internals.Errors;
using Genbox.SimpleS3.Core.Internals.Helpers;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class XmlNamespaceTests
{
    [Fact]
    public void PrefixedErrorDocumentsPreserveServiceErrors()
    {
        using MemoryStream input = new MemoryStream(Encoding.UTF8.GetBytes("<s3:Error xmlns:s3=\"http://s3.amazonaws.com/doc/2006-03-01/\"><s3:Code>NoSuchKey</s3:Code><s3:Message>missing</s3:Message></s3:Error>"));
        using XmlReader reader = XmlHelper.CreateReader(input);
        Assert.Equal(ErrorCode.NoSuchKey, ErrorHandler.Create(reader).Code);
    }

    [Theory]
    [InlineData("<s3:Rule />")]
    [InlineData("<s3:Rule><s3:Other /></s3:Rule>")]
    public void DescendantSearchDoesNotReadValuesFromLaterRules(string rule)
    {
        using MemoryStream input = new MemoryStream(Encoding.UTF8.GetBytes("<s3:Configuration xmlns:s3=\"http://s3.amazonaws.com/doc/2006-03-01/\">" + rule + "<s3:Rule><s3:Value>later</s3:Value></s3:Rule></s3:Configuration>"));
        using XmlReader reader = XmlHelper.CreateReader(input);
        Assert.True(XmlHelper.ReadToDescendant(reader, "Rule"));
        Assert.False(XmlHelper.ReadToDescendant(reader, "Value"));
        Assert.True(XmlHelper.ReadToFollowing(reader, "Value"));
        Assert.Equal("later", reader.ReadElementContentAsString());
    }
}