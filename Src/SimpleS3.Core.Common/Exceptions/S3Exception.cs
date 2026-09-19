using System.Diagnostics.CodeAnalysis;

namespace Genbox.SimpleS3.Core.Common.Exceptions;

/// <summary>Provides the <c>S3Exception</c> API.</summary>
[SuppressMessage("Design", "CA1032", Justification = "Optional arguments already support parameterless and message-only construction while preserving the established named-argument API.")]
public class S3Exception(string? message = null, Exception? innException = null) : Exception(message, innException);