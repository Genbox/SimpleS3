namespace Genbox.SimpleS3.Core.Abstracts.Request;

public interface ISignedRequestHandler
{
    string SignRequest<TReq>(TReq request, TimeSpan expiresIn) where TReq : IRequest;
}