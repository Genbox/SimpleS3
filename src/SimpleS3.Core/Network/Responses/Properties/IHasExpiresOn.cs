namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    public interface IHasExpiresOn
    {
        /// <summary>
        /// If the expiration is configured for the object (see PUT Bucket lifecycle), the response includes this header. It includes the expiry-date
        /// and rule-id key-value pairs that provide information about object expiration. The value of the rule-id is URL encoded.
        /// </summary>
        string ExpiresOn { get; }
    }
}