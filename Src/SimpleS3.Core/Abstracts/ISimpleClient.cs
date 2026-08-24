using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;

namespace Genbox.SimpleS3.Core.Abstracts;

/// <summary>Provides the <c>ISimpleClient</c> API.</summary>
public interface ISimpleClient : IBucketClient, IObjectClient, IMultipartClient, ITransfer, IMultipartTransfer, ISignedClient;