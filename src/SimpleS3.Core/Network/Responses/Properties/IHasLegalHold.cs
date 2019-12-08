namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    public interface IHasLegalHold
    {
        /// <summary>
        /// Specifies whether a legal hold is in effect for this object. This header is only returned if the requester has the s3:GetObjectLegalHold
        /// permission. This header is not returned if the specified version of this object has never had a legal hold applied.
        /// </summary>
        bool LockLegalHold { get; }
    }
}