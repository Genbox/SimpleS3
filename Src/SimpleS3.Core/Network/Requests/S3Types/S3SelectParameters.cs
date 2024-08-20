using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3SelectParameters(string expression, S3InputFormat? inputFormat, S3OutputFormat outputFormat)
{
    public string Expression { get; } = expression;
    public ExpressionType ExpressionType { get; set; }
    public S3InputFormat? InputFormat { get; } = inputFormat;
    public S3OutputFormat OutputFormat { get; } = outputFormat;

    internal void Reset()
    {
        ExpressionType = ExpressionType.Unknown;
        InputFormat?.Reset();
        OutputFormat.Reset();
    }
}