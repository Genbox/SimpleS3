using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Builders;

/// <summary>Provides the <c>ObjectAclBuilder</c> API.</summary>
public class ObjectAclBuilder
{
    /// <summary>Provides the <c>ReadObject</c> API.</summary>
    public AclBuilder ReadObject { get; } = new AclBuilder();
    /// <summary>Provides the <c>ReadAcl</c> API.</summary>
    public AclBuilder ReadAcl { get; } = new AclBuilder();
    /// <summary>Provides the <c>WriteAcl</c> API.</summary>
    public AclBuilder WriteAcl { get; } = new AclBuilder();
    /// <summary>Provides the <c>FullControl</c> API.</summary>
    public AclBuilder FullControl { get; } = new AclBuilder();

    /// <summary>Provides the <c>AddEmail</c> API.</summary>
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

    /// <summary>Provides the <c>AddUserId</c> API.</summary>
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

    /// <summary>Provides the <c>AddGroup</c> API.</summary>
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

    /// <summary>Provides the <c>AddGroup</c> API.</summary>
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