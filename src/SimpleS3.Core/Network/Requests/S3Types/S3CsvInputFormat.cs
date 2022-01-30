using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3CsvInputFormat : S3InputFormat
{
    /// <summary>Indicate if the first line of the file should be used</summary>
    public HeaderUsage HeaderUsage { get; set; }

    /// <summary>The comment character used in the CSV file</summary>
    public char? CommentCharacter { get; set; }

    /// <summary>The character to use when escaping quotes</summary>
    public char? QuoteEscapeCharacter { get; set; }

    /// <summary>Value used to delimit records</summary>
    public char? RecordDelimiter { get; set; }

    /// <summary>Value used to delimit fields</summary>
    public char? FieldDelimiter { get; set; }

    /// <summary>Value used for escaping where the field delimiter is part of the value.</summary>
    public char? QuoteCharacter { get; set; }

    /// <summary>Specifies that CSV field values may contain quoted record delimiters and such records should be allowed.
    /// Default value is FALSE. Setting this value to TRUE may lower performance.</summary>
    public bool? AllowQuotedRecordDelimiter { get; set; }

    internal override void Reset()
    {
        HeaderUsage = HeaderUsage.Unknown;
        CommentCharacter = null;
        QuoteEscapeCharacter = null;
        RecordDelimiter = null;
        FieldDelimiter = null;
        QuoteCharacter = null;
        AllowQuotedRecordDelimiter = null;
    }
}