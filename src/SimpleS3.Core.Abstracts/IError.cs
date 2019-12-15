using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IError
    {
        ErrorCode Code { get; }
        string Message { get; }
        string GetExtraData();
    }
}