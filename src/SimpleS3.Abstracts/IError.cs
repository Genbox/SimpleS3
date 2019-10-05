using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Abstracts
{
    public interface IError
    {
        ErrorCode Code { get; }
        string Message { get; }
        string GetExtraData();
    }
}