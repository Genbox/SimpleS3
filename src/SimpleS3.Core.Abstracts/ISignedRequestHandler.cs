using System;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface ISignedRequestHandler
    {
        string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest;
    }
}