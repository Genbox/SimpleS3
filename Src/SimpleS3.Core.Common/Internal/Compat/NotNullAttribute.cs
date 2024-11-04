#if NETSTANDARD2_0

// ReSharper disable All
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class NotNullAttribute : Attribute
{
    public NotNullAttribute() {}
}
#endif