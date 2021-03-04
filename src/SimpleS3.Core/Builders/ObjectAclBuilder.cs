using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Builders
{
    public class ObjectAclBuilder
    {
        public AclBuilder ReadObject { get; } = new AclBuilder();
        public AclBuilder ReadAcl { get; } = new AclBuilder();
        public AclBuilder WriteAcl { get; } = new AclBuilder();
        public AclBuilder FullControl { get; } = new AclBuilder();

        public ObjectAclBuilder AddEmail(string email, ObjectPermissions permissions)
        {
            if (permissions.HasFlag(ObjectPermissions.Read))
                ReadObject.AddEmail(email);

            if (permissions.HasFlag(ObjectPermissions.ReadAcl))
                ReadAcl.AddEmail(email);

            if (permissions.HasFlag(ObjectPermissions.WriteAcl))
                WriteAcl.AddEmail(email);

            if (permissions.HasFlag(ObjectPermissions.FullControl))
                FullControl.AddEmail(email);

            return this;
        }

        public ObjectAclBuilder AddUserId(string userId, ObjectPermissions permissions)
        {
            if (permissions.HasFlag(ObjectPermissions.Read))
                ReadObject.AddUserId(userId);

            if (permissions.HasFlag(ObjectPermissions.ReadAcl))
                ReadAcl.AddUserId(userId);

            if (permissions.HasFlag(ObjectPermissions.WriteAcl))
                WriteAcl.AddUserId(userId);

            if (permissions.HasFlag(ObjectPermissions.FullControl))
                FullControl.AddUserId(userId);

            return this;
        }

        public ObjectAclBuilder AddGroup(string uri, ObjectPermissions permissions)
        {
            if (permissions.HasFlag(ObjectPermissions.Read))
                ReadObject.AddGroup(uri);

            if (permissions.HasFlag(ObjectPermissions.ReadAcl))
                ReadAcl.AddGroup(uri);

            if (permissions.HasFlag(ObjectPermissions.WriteAcl))
                WriteAcl.AddGroup(uri);

            if (permissions.HasFlag(ObjectPermissions.FullControl))
                FullControl.AddGroup(uri);

            return this;
        }

        public ObjectAclBuilder AddGroup(PredefinedGroup group, ObjectPermissions permissions)
        {
            if (permissions.HasFlag(ObjectPermissions.Read))
                ReadObject.AddGroup(group);

            if (permissions.HasFlag(ObjectPermissions.ReadAcl))
                ReadAcl.AddGroup(group);

            if (permissions.HasFlag(ObjectPermissions.WriteAcl))
                WriteAcl.AddGroup(group);

            if (permissions.HasFlag(ObjectPermissions.FullControl))
                FullControl.AddGroup(group);

            return this;
        }
    }
}