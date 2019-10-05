using System.IO;
using System.Text;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class PutBucketRequestMarshal : IRequestMarshal<PutBucketRequest>
    {
        public Stream MarshalRequest(PutBucketRequest request)
        {
            request.AddHeader(AmzHeaders.XAmzBucketObjectLockEnabled, request.EnableObjectLocking);
            request.AddHeader(AmzHeaders.XAmzAcl, request.Acl);
            request.AddHeader(AmzHeaders.XAmzGrantRead, request.AclGrantRead);
            request.AddHeader(AmzHeaders.XAmzGrantReadAcp, request.AclGrantReadAcp);
            request.AddHeader(AmzHeaders.XAmzGrantWrite, request.AclGrantWrite);
            request.AddHeader(AmzHeaders.XAmzGrantWriteAcp, request.AclGrantWriteAcp);
            request.AddHeader(AmzHeaders.XAmzGrantFullControl, request.AclGrantFullControl);

            if (request.Region != AwsRegion.Unknown)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<CreateBucketConfiguration>");
                sb.Append("<LocationConstraint>").Append(ValueHelper.EnumToString(request.Region)).Append("</LocationConstraint>");
                sb.Append("</CreateBucketConfiguration>");

                return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            }

            return null;
        }
    }
}