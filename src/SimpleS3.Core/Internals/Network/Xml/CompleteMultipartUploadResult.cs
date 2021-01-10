using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Internals.Network.Xml
{
    [XmlRoot]
    internal sealed class CompleteMultipartUploadResult
    {
        public string Location { get; set; }
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string ETag { get; set; }
    }
}