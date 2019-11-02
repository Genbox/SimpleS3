using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart.Xml
{
    [XmlRoot]
    public sealed class CompleteMultipartUploadResult
    {
        public string Location { get; set; }
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string ETag { get; set; }
    }
}