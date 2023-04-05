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
                lookup.Add(name, xmlReader.ReadString());
        }

        string code = lookup["Code"];

        return code switch
        {
            "HeadersNotSigned" => new HeadersNotSignedError(lookup),
            "MethodNotAllowed" => new MethodNotAllowedError(lookup),
            "XAmzContentSHA256Mismatch" => new XAmzContentSha256MismatchError(lookup),
            "InvalidBucketName" => new InvalidBucketNameError(lookup),
            "BucketAlreadyExists" => new BucketAlreadyExistsError(lookup),
            "InvalidArgument" => new InvalidArgumentError(lookup),
            "TooManyBuckets" => new TooManyBucketsError(lookup),
            "NoSuchBucket" => new NoSuchBucketError(lookup),
            "BucketNotEmpty" => new BucketNotEmptyError(lookup),
            "PreconditionFailed" => new PreconditionFailedError(lookup),
            _ => new GenericError(lookup)
        };
    }
}