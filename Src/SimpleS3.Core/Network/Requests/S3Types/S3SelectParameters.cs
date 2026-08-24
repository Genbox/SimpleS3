using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3SelectParameters</c> API.</summary>
public class S3SelectParameters(string expression, S3InputFormat? inputFormat, S3OutputFormat outputFormat)
{
    /// <summary>Provides the <c>Expression</c> API.</summary>
    public string Expression { get; } = expression;
    /// <summary>Provides the <c>ExpressionType</c> API.</summary>
    public ExpressionType ExpressionType { get; set; }
    /// <summary>Provides the <c>InputFormat</c> API.</summary>
    public S3InputFormat? InputFormat { get; } = inputFormat;
    /// <summary>Provides the <c>OutputFormat</c> API.</summary>
    public S3OutputFormat OutputFormat { get; } = outputFormat;

    internal void Reset()
    {
        ExpressionType = ExpressionType.Unknown;
        InputFormat?.Reset();
        OutputFormat.Reset();
    }
}