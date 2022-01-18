using System.Xml;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Errors;

namespace Genbox.SimpleS3.Core.Internals.Errors;

internal static class ErrorHandler
{
    internal static GenericError Create(Stream response)
    {
        Dictionary<string, string> lookup = new Dictionary<string, string>(StringComparer.Ordinal);

        using (XmlReader xmlReader = new XmlTextReader(response))
        {
            xmlReader.ReadToDescendant("Error");

            foreach (string name in XmlHelper.ReadElements(xmlReader))
            {
                lookup.Add(name, xmlReader.ReadString());
            }
        }

        string code = lookup["Code"];

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
            case "PreconditionFailed":
                return new PreconditionFailedError(lookup);
            default:
                return new GenericError(lookup);
        }
    }
}