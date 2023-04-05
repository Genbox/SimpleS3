// ReSharper disable CheckNamespace
// ReSharper disable UnusedParameter.Local
// ReSharper disable EmptyConstructor

#if NETSTANDARD2_0
#pragma warning disable MA0048, CA1019, S3376, CA1710

namespace System.Diagnostics.CodeAnalysis;

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

#pragma warning restore MA0048, CA1019, S3376, CA1710
#endif