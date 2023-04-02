using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3SelectParameters
{
    public S3SelectParameters(string expression, S3InputFormat? inputFormat, S3OutputFormat outputFormat)
    {
        Expression = expression;
        InputFormat = inputFormat;
        OutputFormat = outputFormat;
    }

    public string Expression { get; }
    public ExpressionType ExpressionType { get; set; }
    public S3InputFormat? InputFormat { get; }
    public S3OutputFormat OutputFormat { get; }

    internal void Reset()
    {
        ExpressionType = ExpressionType.Unknown;
        InputFormat?.Reset();
        OutputFormat.Reset();
    }
}