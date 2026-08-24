using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Validation;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors;

/// <summary>Provides the <c>GenericError</c> API.</summary>
[PublicAPI]
public class GenericError : IError
{
    /// <summary>Provides the <c>GenericError</c> API.</summary>
    public GenericError(IDictionary<string, string> lookup)
    {
        Validator.RequireNotNull(lookup);

        Code = Abstracts.Enums.Enums.ErrorCode.Parse(lookup["Code"]);
        Message = lookup["Message"];

        Data = new Dictionary<string, string>(lookup.Count, StringComparer.OrdinalIgnoreCase);

        foreach (KeyValuePair<string, string> pair in lookup)
        {
            if (pair.Key is "Message" or "Code")
                continue;

            Data.Add(pair);
        }
    }

    /// <summary>Provides the <c>Code</c> API.</summary>
    public ErrorCode Code { get; }
    /// <summary>Provides the <c>Message</c> API.</summary>
    public string Message { get; }
    /// <summary>Provides the <c>Data</c> API.</summary>
    public IDictionary<string, string> Data { get; }

    /// <summary>Provides the <c>GetErrorDetails</c> API.</summary>
    public virtual string GetErrorDetails() => string.Empty;
}