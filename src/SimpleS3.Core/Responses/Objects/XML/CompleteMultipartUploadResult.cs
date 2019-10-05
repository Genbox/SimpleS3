using System.Xml.Serialization;

namespace Genbox.SimpleS3.Core.Responses.Objects.XML
{
    [XmlRoot]
    public class CompleteMultipartUploadResult
    {
        public string Location { get; set; }
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string ETag { get; set; }
    }
}