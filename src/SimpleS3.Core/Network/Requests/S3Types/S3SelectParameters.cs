using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public class S3SelectParameters
    {
        public S3SelectParameters() { }

        public S3SelectParameters(string expression, InputFormat inputFormat, OutputFormat outputFormat)
        {
            Expression = expression;
            InputFormat = inputFormat;
            OutputFormat = outputFormat;
        }

        public string? Expression { get; set; }
        public ExpressionType ExpressionType { get; set; }
        public InputFormat? InputFormat { get; set; }
        public OutputFormat? OutputFormat { get; set; }

        internal void Reset()
        {
            Expression = null;
            ExpressionType = ExpressionType.Unknown;
            InputFormat?.Reset();
            OutputFormat?.Reset();
        }
    }
}