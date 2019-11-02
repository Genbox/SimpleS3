namespace Genbox.SimpleS3.Core.Network.Requests.Properties
{
    public interface IHasContentMd5
    {
        /// <summary>
        /// 128-bit MD5 digest of the data. This property can be used as a message integrity check to
        /// verify that the data is the same data that was originally sent. Although it is optional, we recommend using the Content-MD5 mechanism as an
        /// end-to-end integrity check.
        /// </summary>
        byte[] ContentMd5 { get; set; }
    }
}