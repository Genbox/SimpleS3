using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Buckets
{
    public class DeleteBucketRequestValidator : RequestWithoutObjectKeyBase<DeleteBucketRequest>
    {
        public DeleteBucketRequestValidator(IOptions<S3Config> config) : base(config)
        {
        }
    }
}