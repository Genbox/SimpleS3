using System.Collections.Generic;
using Genbox.SimpleS3.Core.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Responses.Objects
{
    public class DeleteObjectsResponse : BaseResponse
    {
        public IList<S3DeletedObject> Deleted { get; set; }
        public IList<S3DeleteError> Errors { get; set; }
    }
}