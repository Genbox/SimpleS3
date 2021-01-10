using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot]
    internal sealed class InitiateMultipartUploadResult
    {
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string UploadId { get; set; }
    }
}