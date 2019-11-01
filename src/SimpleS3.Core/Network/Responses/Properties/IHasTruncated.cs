using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    public interface IHasTruncated
    {
        /// <summary>
        /// Indicates whether the returned list of multipart uploads is truncated. A value of true indicates that the list was truncated. The list can
        /// be truncated if the number of multipart uploads exceeds the limit allowed or specified by max uploads.
        /// </summary>
        bool IsTruncated { get; }

        /// <summary>
        /// Encoding type used by Amazon S3 to encode object key names in the XML response. If you specify the encoding-type request parameter, Amazon
        /// S3 includes this element in the response, and returns encoded key name values in the following response elements: <see cref="Delimiter" />,
        /// <see cref="Prefix" />, <see cref="S3Object.ObjectKey" />, and <see cref="StartAfter" />.
        /// </summary>
        EncodingType EncodingType { get; }
    }
}