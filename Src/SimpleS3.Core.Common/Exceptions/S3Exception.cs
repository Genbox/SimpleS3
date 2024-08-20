namespace Genbox.SimpleS3.Core.Common.Exceptions;

public class S3Exception(string? message = null, Exception? innException = null) : Exception(message, innException);