using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    public interface IPreSignedObjectOperations
    {
        Task<string> SignGetObjectAsync(GetObjectRequest request, CancellationToken token = default);
    }
}