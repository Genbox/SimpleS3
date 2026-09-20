#if NETSTANDARD2_0

// Justification: This compatibility shim mirrors the framework attribute shape on target frameworks that do not provide it.
// ReSharper disable All
namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class DoesNotReturnIfAttribute : Attribute
{
    public DoesNotReturnIfAttribute(bool parameterValue)
    {
        ParameterValue = parameterValue;
    }

    public bool ParameterValue { get; }
}
#endif