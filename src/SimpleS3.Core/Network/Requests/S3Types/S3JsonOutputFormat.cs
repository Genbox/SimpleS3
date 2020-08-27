namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public class S3JsonOutputFormat : OutputFormat
    {
        public S3JsonOutputFormat(string recordDelimiter)
        {
            RecordDelimiter = recordDelimiter;
        }

        public string RecordDelimiter { get; set; }

        internal override void Reset()
        {
            RecordDelimiter = string.Empty;
        }
    }
}