using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public class S3CsvOutputFormat : S3OutputFormat
    {
        /// <summary>Value used to delimit fields</summary>
        public char? FieldDelimiter { get; set; }

        /// <summary>Value used for escaping where the field delimiter is part of the value</summary>
        public char? QuoteCharacter { get; set; }

        /// <summary>The character to use when escaping quotes</summary>
        public char? QuoteEscapeCharacter { get; set; }

        /// <summary>Indicates whether or not all output fields should be quoted</summary>
        public QuoteField QuoteFields { get; set; }

        /// <summary>Value used to delimit records</summary>
        public char? RecordDelimiter { get; set; }

        internal override void Reset()
        {
            FieldDelimiter = null;
            QuoteCharacter = null;
            QuoteEscapeCharacter = null;
            QuoteFields = QuoteField.Unknown;
            RecordDelimiter = null;
        }
    }
}