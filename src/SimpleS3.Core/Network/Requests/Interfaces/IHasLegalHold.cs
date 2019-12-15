namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces
{
    public interface IHasLegalHold
    {
        /// <summary>Specifies whether a legal hold will be applied to this object.</summary>
        bool? LockLegalHold { get; set; }
    }
}