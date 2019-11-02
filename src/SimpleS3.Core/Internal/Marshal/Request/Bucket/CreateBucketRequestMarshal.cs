using System.IO;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Bucket
{
    [UsedImplicitly]
    internal class CreateBucketRequestMarshal : IRequestMarshal<CreateBucketRequest>
    {
        public Stream MarshalRequest(CreateBucketRequest request, IS3Config config)
        {
            request.AddHeader(AmzHeaders.XAmzBucketObjectLockEnabled, request.EnableObjectLocking);
            request.AddHeader(AmzHeaders.XAmzAcl, request.Acl);
            request.AddHeader(AmzHeaders.XAmzGrantRead, request.AclGrantRead);
            request.AddHeader(AmzHeaders.XAmzGrantReadAcp, request.AclGrantReadAcp);
            request.AddHeader(AmzHeaders.XAmzGrantWrite, request.AclGrantWrite);
            request.AddHeader(AmzHeaders.XAmzGrantWriteAcp, request.AclGrantWriteAcp);
            request.AddHeader(AmzHeaders.XAmzGrantFullControl, request.AclGrantFullControl);

            //Hardcore the LocationConstraint to the region from the config
            StringBuilder sb = new StringBuilder(120);
            sb.Append("<CreateBucketConfiguration>");
            sb.Append("<LocationConstraint>").Append(ValueHelper.EnumToString(config.Region)).Append("</LocationConstraint>");
            sb.Append("</CreateBucketConfiguration>");

            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }
    }
}