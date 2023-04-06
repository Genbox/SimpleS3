using System.Text;
using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Builders;

/// <summary>Builder to help you design an access control list (ACL). For more information on ACLs, see
/// https://docs.aws.amazon.com/AmazonS3/latest/dev/acl-overview.html</summary>
public class AclBuilder : IHttpHeaderBuilder
{
    private HashSet<string>? _emails;
    private HashSet<string>? _ids;
    private StringBuilder? _sb;
    private HashSet<string>? _uris;

    public string? HeaderName => null;

    public string? Build()
    {
        if (!HasData())
            return null;

        int count = 0;

        if (_emails != null)
            count += _emails.Count;

        if (_ids != null)
            count += _ids.Count;

        if (_uris != null)
            count += _uris.Count;

        if (_sb == null)
            _sb = new StringBuilder(100);
        else
            _sb.Clear();

        if (_emails != null && _emails.Count > 0)
        {
            foreach (string email in _emails)
            {
                _sb.Append("emailAddress=\"").Append(email).Append('"');

                if (--count != 0)
                    _sb.Append(',');
            }
        }

        if (_ids != null && _ids.Count > 0)
        {
            foreach (string id in _ids)
            {
                _sb.Append("id=\"").Append(id).Append('"');

                if (--count != 0)
                    _sb.Append(',');
            }
        }

        if (_uris != null && _uris.Count > 0)
        {
            foreach (string uri in _uris)
            {
                _sb.Append("uri=\"").Append(uri).Append('"');

                if (--count != 0)
                    _sb.Append(',');
            }
        }

        return _sb.ToString();
    }

    public void Reset()
    {
        _emails?.Clear();
        _ids?.Clear();
        _uris?.Clear();
    }

    public bool HasData() => (_emails != null && _emails.Count > 0) || (_ids != null && _ids.Count > 0) || (_uris != null && _uris.Count > 0);

    /// <summary>Add an email to the ACL. Note that email support is only in these AWS regions: <list type="bullet">
    ///     <item>
    ///         <term>US East (N. Virginia)</term>
    ///     </item> <item>
    ///         <term>US West (N. California)</term>
    ///     </item> <item>
    ///         <term>US West (Oregon)</term>
    ///     </item> <item>
    ///         <term>Asia Pacific (Singapore)</term>
    ///     </item> <item>
    ///         <term>Asia Pacific (Sydney)</term>
    ///     </item> <item>
    ///         <term>Asia Pacific (Tokyo)</term>
    ///     </item> <item>
    ///         <term>EU (Ireland)</term>
    ///     </item> <item>
    ///         <term>South America (São Paulo)</term>
    ///     </item>
    /// </list></summary>
    /// <param name="email">The email you want to add</param>
    public AclBuilder AddEmail(string email)
    {
        if (_emails == null)
            _emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!_emails.Add(email))
            throw new ArgumentException($"You already added the email {email}");

        return this;
    }

    /// <summary>Add a user id to the ACL. See
    /// https://docs.aws.amazon.com/general/latest/gr/acct-identifiers.html#FindingCanonicalId on how you find the user id
    /// associated with a user.</summary>
    /// <param name="id">The user id, for example 79a59df900b949e55d96a1e698fbacedfd6e09d98eacf8f8d5218e7cd47ef2be</param>
    public AclBuilder AddUserId(string id)
    {
        Validator.RequireNotNullOrWhiteSpace(id);

        if (_ids == null)
            _ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (id.Length != 64)
            throw new ArgumentException($"The user id {id} is not 64 characters long");

        if (!_ids.Add(id))
            throw new ArgumentException($"You already added the id {id}");

        return this;
    }

    /// <summary>Grant permission to a predefined Amazon S3 group.</summary>
    /// <param name="uri">An URI to the group</param>
    public AclBuilder AddGroup(string uri)
    {
        Validator.RequireNotNullOrWhiteSpace(uri);

        if (_uris == null)
            _uris = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!_uris.Add(uri))
            throw new ArgumentException($"You already added the URI {uri}");

        return this;
    }

    /// <summary>Grant permission to a predefined Amazon S3 group.</summary>
    /// <param name="group">One of Amazon's predefined groups</param>
    public AclBuilder AddGroup(PredefinedGroup group)
    {
        Validator.RequireValidEnum(group);

        string groupStr = EnumHelper.AsString(group);

        Validator.RequireNotNull(groupStr, "Bug: PredefinedGroup is missing EnumValue");

        return AddGroup(groupStr!);
    }
}