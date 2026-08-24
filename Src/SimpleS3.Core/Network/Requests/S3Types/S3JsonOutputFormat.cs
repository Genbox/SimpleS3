namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3JsonOutputFormat</c> API.</summary>
public class S3JsonOutputFormat(string recordDelimiter) : S3OutputFormat
{
    /// <summary>Provides the <c>RecordDelimiter</c> API.</summary>
    public string RecordDelimiter { get; set; } = recordDelimiter;

    internal override void Reset()
    {
        RecordDelimiter = string.Empty;
    }
}