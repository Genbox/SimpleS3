#if NETSTANDARD2_0

// ReSharper disable All
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class NotNullAttribute : Attribute
{
    public NotNullAttribute() {}
}
#endif