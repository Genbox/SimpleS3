using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart
{
    /// <summary>
    /// This operation lists in-progress multipart uploads. An in-progress multipart upload is a multipart upload that has been initiated using the
    /// Initiate Multipart Upload request, but has not yet been completed or aborted. This operation returns at most 1,000 multipart uploads in the response.
    /// 1,000 multipart uploads is the maximum number of uploads a response can include, which is also the default value. You can further limit the number of
    /// uploads in a response by specifying the max-uploads parameter in the response. If additional multipart uploads satisfy the list criteria, the
    /// response will contain an IsTruncated element with the value true. To list the additional multipart uploads, use the key-marker and upload-id-marker
    /// request parameters. In the response, the uploads are sorted by key. If your application has initiated more than one multipart upload using the same
    /// object key, then uploads in the response are first sorted by key. Additionally, uploads are sorted in ascending order within each key by the upload
    /// initiation time.
    /// </summary>
    public class ListMultipartUploadsRequest : BaseRequest
    {
        public ListMultipartUploadsRequest(string bucketName) : base(HttpMethod.GET, bucketName, string.Empty)
        {
        }

        /// <summary>
        /// Character you use to group keys. All keys that contain the same string between the prefix, if specified, and the first occurrence of the
        /// delimiter after the prefix are grouped under a single result element, CommonPrefixes. If you don't specify the prefix parameter, then the substring
        /// starts at the beginning of the key. The keys that are grouped under CommonPrefixes result element are not returned elsewhere in the response.
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// Requests Amazon S3 to encode the response and specifies the encoding method to use. An object key can contain any Unicode character;
        /// however, XML 1.0 parser cannot parse some characters, such as characters with an ASCII value from 0 to 10. For characters that are not supported in
        /// XML 1.0, you can add this parameter to request that Amazon S3 encode the keys in the response.
        /// </summary>
        public EncodingType EncodingType { get; set; }

        /// <summary>
        /// Sets the maximum number of multipart uploads, from 1 to 1000, to return in the response body. 1000 is the maximum number of uploads that can
        /// be returned in a response. Default is 1000.
        /// </summary>
        public int? MaxUploads { get; set; }

        /// <summary>
        /// Together with upload-id-marker, this parameter specifies the multipart upload after which listing should begin. If upload-id-marker is not
        /// specified, only the keys lexicographically greater than the specified key-marker will be included in the list. If upload-id-marker is specified, any
        /// multipart uploads for a key equal to the key-marker might also be included, provided those multipart uploads have upload IDs lexicographically
        /// greater than the specified upload-id-marker.
        /// </summary>
        public string KeyMarker { get; set; }

        /// <summary>
        /// Lists in-progress uploads only for those keys that begin with the specified prefix. You can use prefixes to separate a bucket into different
        /// grouping of keys. (You can think of using prefix to make groups in the same way you'd use a folder in a file system.)
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Together with key-marker, specifies the multipart upload after which listing should begin. If key-marker is not specified, the
        /// upload-id-marker parameter is ignored. Otherwise, any multipart uploads for a key equal to the key-marker might be included in the list only if they
        /// have an upload ID lexicographically greater than the specified upload-id-marker.
        /// </summary>
        public string UploadIdMarker { get; set; }
    }
}