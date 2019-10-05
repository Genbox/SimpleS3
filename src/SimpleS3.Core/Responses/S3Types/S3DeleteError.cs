namespace Genbox.SimpleS3.Core.Responses.S3Types
{
    public class S3DeleteError
    {
        public string Key { get; set; }
        public string VersionId { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}