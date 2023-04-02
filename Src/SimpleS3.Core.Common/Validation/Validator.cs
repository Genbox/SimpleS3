using System.Collections;
using System.Runtime.CompilerServices;
using NoEnumeration = JetBrains.Annotations.NoEnumerationAttribute;

namespace Genbox.SimpleS3.Core.Common.Validation;

#if NETSTANDARD2_0
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CallerArgumentExpressionAttribute : Attribute
{
    public CallerArgumentExpressionAttribute(string notUsed) {}
}

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class DoesNotReturnIf : Attribute
{
    public DoesNotReturnIf(bool notUsed) {}
}

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class NotNull : Attribute
{
    public NotNull() {}
}
#endif

// ReSharper disable ExplicitCallerInfoArgument
public static class Validator
{
    public static void RequireValidEnum<T>(T enumVal, string? message = null, bool defaultValueAllowed = false, [CallerArgumentExpression("enumVal")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null) where T : struct, Enum
    {
#if NETSTANDARD2_0
        RequireThat(Enum.IsDefined(typeof(T), enumVal), message, callerArgument, callerMember, lineNumber);
#else
        RequireThat(Enum.IsDefined(enumVal), message, callerArgument, callerMember, lineNumber);
#endif

        if (!defaultValueAllowed)
            RequireThat(!enumVal.Equals(default(T)), message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireValidEnum<T>([NotNull]string? value, out T enumVal, string? message = null, bool defaultValueAllowed = false, [CallerArgumentExpression("enumVal")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null) where T : struct, Enum
    {
        RequireNotNull(value, message, callerArgument, callerMember, lineNumber);
        RequireThat(Enum.TryParse(value, true, out enumVal), message, callerArgument, callerMember, lineNumber);

        if (!defaultValueAllowed)
            RequireThat(!enumVal.Equals(default(T)), message, callerArgument, callerMember, lineNumber);
    }

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
        RequireThat(value!.Count > 0, message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireNotNullOrEmpty([NotNull]string? value, string? message = null, [CallerArgumentExpression("value")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null)
    {
        RequireThat(!string.IsNullOrEmpty(value), message, callerArgument, callerMember, lineNumber);
    }

    public static void RequireValueAndItemsNotNull([NotNull]ICollection? value, string? message = null, [CallerArgumentExpression("value")]string? callerArgument = null, [CallerMemberName]string? callerMember = null, [CallerLineNumber]int? lineNumber = null)
    {
        RequireNotNull(value, message, callerArgument, callerMember, lineNumber);
        RequireThat(!AnyItemNull(value!), message, callerArgument, callerMember, lineNumber);
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