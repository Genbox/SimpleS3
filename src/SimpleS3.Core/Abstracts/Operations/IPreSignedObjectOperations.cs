using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    public interface IPreSignedObjectOperations
    {
        Task<string> SignGetObjectAsync(GetObjectRequest request, TimeSpan expiresIn, CancellationToken token = default);
        Task<string> SignDeleteObjectAsync(DeleteObjectRequest request, TimeSpan expiresIn, CancellationToken token = default);
        Task<string> SignPutObjectAsync(PutObjectRequest request, TimeSpan expiresIn, CancellationToken token = default);
        Task<string> SignHeadObjectAsync(HeadObjectRequest request, TimeSpan expiresIn, CancellationToken token = default);
    }
}