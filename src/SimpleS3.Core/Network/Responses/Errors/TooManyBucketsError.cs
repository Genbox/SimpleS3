using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors
{
    [PublicAPI]
    public class TooManyBucketsError : GenericError
    {
        internal TooManyBucketsError(IDictionary<string, string> lookup) : base(lookup)
        {
            CurrentNumberOfBuckets = int.Parse(lookup["CurrentNumberOfBuckets"], NumberFormatInfo.InvariantInfo);
            AllowedNumberOfBuckets = int.Parse(lookup["AllowedNumberOfBuckets"], NumberFormatInfo.InvariantInfo);
        }

        public int CurrentNumberOfBuckets { get; }
        public int AllowedNumberOfBuckets { get; }

        public override string GetErrorDetails()
        {
            return $"CurrentNumberOfBuckets: {CurrentNumberOfBuckets} - AllowedNumberOfBuckets: {AllowedNumberOfBuckets}";
        }
    }
}