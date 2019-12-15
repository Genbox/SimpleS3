using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Internals.Helpers;
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