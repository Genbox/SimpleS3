#if NETSTANDARD2_0

// Justification: This compatibility shim mirrors the framework attribute shape on target frameworks that do not provide it.
// ReSharper disable All
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class NotNullAttribute : Attribute
{
    public NotNullAttribute() {}
}
#endif