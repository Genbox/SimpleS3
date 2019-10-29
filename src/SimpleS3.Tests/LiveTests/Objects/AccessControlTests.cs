using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Fluent;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class AccessControlTests : LiveTestBase
    {
        public AccessControlTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Theory]
        [InlineData(ObjectCannedAcl.AuthenticatedRead)]
        [InlineData(ObjectCannedAcl.AwsExecRead)]
        [InlineData(ObjectCannedAcl.BucketOwnerFullControl)]
        [InlineData(ObjectCannedAcl.BucketOwnerRead)]
        [InlineData(ObjectCannedAcl.Private)]
        [InlineData(ObjectCannedAcl.PublicRead)]
        [InlineData(ObjectCannedAcl.PublicReadWrite)]
        public Task CannedAcl(ObjectCannedAcl acl)
        {
            return UploadAsync($"{nameof(CannedAcl)}-{acl}", request => request.Acl = acl);
        }

        [Theory]
        [InlineData(ObjectCannedAcl.AuthenticatedRead)]
        [InlineData(ObjectCannedAcl.AwsExecRead)]
        [InlineData(ObjectCannedAcl.BucketOwnerFullControl)]
        [InlineData(ObjectCannedAcl.BucketOwnerRead)]
        [InlineData(ObjectCannedAcl.Private)]
        [InlineData(ObjectCannedAcl.PublicRead)]
        [InlineData(ObjectCannedAcl.PublicReadWrite)]
        public Task CannedAclFluent(ObjectCannedAcl acl)
        {
            return UploadTransferAsync($"{nameof(CannedAclFluent)}-{acl}", upload => upload.WithAccessControl(acl));
        }

        [Fact]
        public async Task MultiplePermissions()
        {
            await UploadAsync(nameof(MultiplePermissions), request =>
            {
                request.AclGrantRead.AddEmail(TestConstants.TestEmail);
                request.AclGrantRead.AddUserId(TestConstants.TestUserId);
                request.AclGrantReadAcp.AddEmail(TestConstants.TestEmail);
                request.AclGrantReadAcp.AddUserId(TestConstants.TestUserId);
                request.AclGrantWriteAcp.AddEmail(TestConstants.TestEmail);
                request.AclGrantWriteAcp.AddUserId(TestConstants.TestUserId);
                request.AclGrantFullControl.AddEmail(TestConstants.TestEmail);
                request.AclGrantFullControl.AddUserId(TestConstants.TestUserId);
            }).ConfigureAwait(false);
        }

        [Fact]
        public Task MultiplePermissionsFluent()
        {
            ObjectAclBuilder acl = new ObjectAclBuilder();
            acl.AddEmail(TestConstants.TestEmail, ObjectPermissions.Read | ObjectPermissions.ReadAcl | ObjectPermissions.WriteAcl | ObjectPermissions.FullControl);
            acl.AddUserId(TestConstants.TestUserId, ObjectPermissions.Read | ObjectPermissions.ReadAcl | ObjectPermissions.WriteAcl | ObjectPermissions.FullControl);

            return UploadTransferAsync(nameof(MultiplePermissionsFluent), upload => upload.WithAccessControl(acl));
        }
    }
}