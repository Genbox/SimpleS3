using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NoEnumeration = JetBrains.Annotations.NoEnumerationAttribute;

namespace Genbox.SimpleS3.Core.Common.Validation;

// ReSharper disable ExplicitCallerInfoArgument
public static class Validator
{
    public static void RequireNotNull<T>([NotNull]T? value, string? message = null, [CallerArgumentExpression("value")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null) where T : class
    {
        RequireThat(value != null, message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireNotNull<T>([NotNull][NoEnumeration]IEnumerable<T>? value, string? message = null, [CallerArgumentExpression("value")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null)
    {
        RequireThat(value != null, message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireNotNullOrEmpty([NotNull]ICollection? value, string? message = null, [CallerArgumentExpression("value")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null)
    {
        RequireNotNull(value, message, callerArgument, callerMember, lineNumber);
        RequireThat(value.Count > 0, message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireNotNullOrEmpty([NotNull]string? value, string? message = null, [CallerArgumentExpression("value")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null)
    {
        RequireThat(!string.IsNullOrEmpty(value), message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireValueAndItemsNotNull([NotNull]ICollection? value, string? message = null, [CallerArgumentExpression("value")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null)
    {
        RequireNotNull(value, message, callerArgument, callerMember, lineNumber);
        RequireThat(!AnyItemNull(value), message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireNotNullOrWhiteSpace([NotNull]string? value, string? message = null, [CallerArgumentExpression("value")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null)
    {
        RequireThat(!string.IsNullOrWhiteSpace(value), message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireThat([DoesNotReturnIf(false)]bool condition, string? message = null, [CallerArgumentExpression("condition")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null)
    {
        if (!condition)
            throw new RequireException(callerArgument!, callerMember!, lineNumber!.Value, message);
    }

    private static bool AnyItemNull(ICollection value)
    {
        foreach (object o in value)
        {
            if (o == null)
                return true;
        }

        return false;
    }
}