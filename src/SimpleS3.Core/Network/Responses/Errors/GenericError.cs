using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Helpers;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

[PublicAPI]
public class GenericError : IError
{
    public GenericError(IDictionary<string, string> lookup)
    {
        Validator.RequireNotNull(lookup, nameof(lookup));

        Code = ValueHelper.ParseEnum<ErrorCode>(lookup["Code"]);
        Message = lookup["Message"];

        Data = new Dictionary<string, string>(lookup.Count, StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, string> pair in lookup)
        {
            if (pair.Key == "Message" || pair.Key == "Code")
                continue;

            Data.Add(pair);
        }
    }

    public ErrorCode Code { get; }
    public string Message { get; }
    public IDictionary<string, string> Data { get; }

    public virtual string GetErrorDetails()
    {
        return string.Empty;
    }
}