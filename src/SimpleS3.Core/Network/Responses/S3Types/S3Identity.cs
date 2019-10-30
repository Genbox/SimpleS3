namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Identity
    {
        /// <summary>
        /// The display name of the identity
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The unique identifier of the identity
        /// </summary>
        public string Id { get; internal set; }
    }
}