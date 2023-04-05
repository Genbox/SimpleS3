using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Common.Validation;

public static class ValidationMessages
{
    public static IDictionary<ValidationStatus, string> Messages { get; } = new Dictionary<ValidationStatus, string>
    {
        { ValidationStatus.Ok, string.Empty },
        { ValidationStatus.WrongFormat, "The input was not in the correct format. The disallowed value was '{0}'" },
        { ValidationStatus.WrongLength, "The input was not the correct length. Length should be '{0}'" },
        { ValidationStatus.NullInput, "You supplied a null input where it is not allowed" },
        { ValidationStatus.ReservedName, "You supplied a name that is reserved. Not allowed: '{0}'" },
        { ValidationStatus.Unknown, "An unknown error occurred" }
    };

    public static string GetMessage(ValidationStatus status, string? disallowed)
    {
        string message = Messages[status];

        if (disallowed != null)
            message = string.Format(CultureInfo.InvariantCulture, message, disallowed);

        return message;
    }
}