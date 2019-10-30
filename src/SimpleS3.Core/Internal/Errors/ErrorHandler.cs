using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Genbox.SimpleS3.Core.Network.Responses.Errors;

namespace Genbox.SimpleS3.Core.Internal.Errors
{
    internal static class ErrorHandler
    {
        internal static GenericError Create(Stream response)
        {
            XDocument errorDocument = XDocument.Load(response);

            if (errorDocument.Root == null)
                throw new Exception("Unknown format in error response");

            Dictionary<string, string> lookup = errorDocument.Root.Elements().ToDictionary(x => x.Name.LocalName, x => x.Value, StringComparer.OrdinalIgnoreCase);

            string code = lookup["code"];

            switch (code)
            {
                case "HeadersNotSigned":
                    return new HeadersNotSignedError(lookup);
                case "MethodNotAllowed":
                    return new MethodNotAllowedError(lookup);
                case "XAmzContentSHA256Mismatch":
                    return new XAmzContentSha256MismatchError(lookup);
                case "InvalidBucketName":
                    return new InvalidBucketNameError(lookup);
                case "BucketAlreadyExists":
                    return new BucketAlreadyExistsError(lookup);
                case "InvalidArgument":
                    return new InvalidArgumentError(lookup);
                case "TooManyBuckets":
                    return new TooManyBucketsError(lookup);
                case "NoSuchBucket":
                    return new NoSuchBucketError(lookup);
                case "BucketNotEmpty":
                    return new BucketNotEmptyError(lookup);
                default:
                    return new GenericError(lookup);
            }
        }
    }
}