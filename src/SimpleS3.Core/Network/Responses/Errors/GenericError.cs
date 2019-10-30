using System.Collections.Generic;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Utils;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors
{
    [PublicAPI]
    public class GenericError : IError
    {
        public GenericError(IDictionary<string, string> lookup)
        {
            Validator.RequireNotNull(lookup, nameof(lookup));

            Code = ValueHelper.ParseEnum<ErrorCode>(lookup["Code"]);
            Message = lookup["Message"];
        }

        public ErrorCode Code { get; }
        public string Message { get; }

        public virtual string GetExtraData()
        {
            return string.Empty;
        }
    }
}