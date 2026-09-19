using System.Diagnostics.CodeAnalysis;

namespace Genbox.SimpleS3.Core.Abstracts.Features;

/// <summary>Provides the <c>ISupportStreaming</c> API.</summary>
[SuppressMessage("Design", "CA1040", Justification = "This marker interface identifies a capability or provides the non-generic base used by dependency injection and marshal discovery.")]
public interface ISupportStreaming;