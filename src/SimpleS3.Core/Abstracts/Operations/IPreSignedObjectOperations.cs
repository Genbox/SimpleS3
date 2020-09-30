using System;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    public interface IPreSignedObjectOperations
    {
        string SignGetObject(GetObjectRequest request, TimeSpan expiresIn);
        string SignDeleteObject(DeleteObjectRequest request, TimeSpan expiresIn);
        string SignPutObject(PutObjectRequest request, TimeSpan expiresIn);
        string SignHeadObject(HeadObjectRequest request, TimeSpan expiresIn);
    }
}