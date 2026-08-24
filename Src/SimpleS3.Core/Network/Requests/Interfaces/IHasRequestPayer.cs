using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

/// <summary>Provides the <c>IHasRequestPayer</c> API.</summary>
public interface IHasRequestPayer
{
    /// <summary>When true, confirms that the requester knows that she or he will be charged for the request. Bucket owners need not specify this parameter in their requests.</summary>
    Payer RequestPayer { get; set; }
}