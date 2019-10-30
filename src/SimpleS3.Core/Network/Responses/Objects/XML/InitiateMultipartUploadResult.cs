using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects.XML
{
    [XmlRoot]
    public sealed class InitiateMultipartUploadResult
    {
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string UploadId { get; set; }
    }
}