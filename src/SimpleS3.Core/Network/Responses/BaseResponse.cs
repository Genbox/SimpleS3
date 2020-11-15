using System;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Network.Responses
{
    public abstract class BaseResponse : IResponse
    {
        public int StatusCode { get; set; }

        public IError? Error { get; set; }

        public long ContentLength { get; set; }

        public bool ConnectionClosed { get; set; }

        public DateTimeOffset Date { get; set; }

        public string? Server { get; set; }

        public string? ResponseId { get; set; }

        public string? RequestId { get; set; }

        public bool IsSuccess { get; set; }
    }
}