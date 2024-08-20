namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3JsonOutputFormat(string recordDelimiter) : S3OutputFormat
{
    public string RecordDelimiter { get; set; } = recordDelimiter;

    internal override void Reset()
    {
        RecordDelimiter = string.Empty;
    }
}