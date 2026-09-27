using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;

namespace Genbox.SimpleS3.Core.Abstracts;

/// <summary>Exposes the complete S3 API across providers. Calling an unsupported operation through this interface throws <see cref="NotSupportedException" />.</summary>
public interface ISimpleClient : IBucketClient, IObjectClient, IMultipartClient, ISignedClient, ITransfer, IMultipartTransfer;