namespace Genbox.SimpleS3.Core.Network.SharedProperties
{
    public interface IHasVersionId
    {
        /// <summary>
        /// The version of the object. When you enable versioning, S3 generates a random number for objects added to a bucket. When you put an object in
        /// a bucket where versioning has been suspended, <see cref="VersionId" /> is always null.
        /// </summary>
        string VersionId { get; }
    }
}
