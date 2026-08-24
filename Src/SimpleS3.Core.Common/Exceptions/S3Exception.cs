namespace Genbox.SimpleS3.Core.Common.Exceptions;

/// <summary>Provides the <c>S3Exception</c> API.</summary>
public class S3Exception(string? message = null, Exception? innException = null) : Exception(message, innException);