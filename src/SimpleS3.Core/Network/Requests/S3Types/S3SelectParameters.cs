using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public class S3SelectParameters
    {
        public string Expression { get; set; }
        public ExpressionType ExpressionType { get; set; }
        public InputFormat InputFormat { get; set; }
        public OutputFormat OutputFormat { get; set; }
    }
}