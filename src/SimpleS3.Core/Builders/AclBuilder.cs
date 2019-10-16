using System;
using System.Collections.Generic;
using System.Text;
using EnumsNET;
using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Utils;

namespace Genbox.SimpleS3.Core.Builders
{
    /// <summary>
    /// Builder to help you design an access control list (ACL). For more information on ACLs, see
    /// https://docs.aws.amazon.com/AmazonS3/latest/dev/acl-overview.html
    /// </summary>
    public class AclBuilder : IHttpHeaderBuilder
    {
        private ISet<string> _emails;
        private ISet<string> _ids;
        private ISet<string> _uris;

        public string Build()
        {
            if (_emails == null && _ids == null && _uris == null)
                return null;

            int count = 0;

            if (_emails != null)
                count += _emails.Count;

            if (_ids != null)
                count += _ids.Count;

            if (_uris != null)
                count += _uris.Count;

            StringBuilder sb = new StringBuilder(count * 50);

            if (_emails != null)
            {
                foreach (string email in _emails)
                {
                    sb.Append("emailAddress=\"").Append(email).Append('"');

                    if (--count != 0)
                        sb.Append(',');
                }
            }

            if (_ids != null)
            {
                foreach (string id in _ids)
                {
                    sb.Append("id=\"").Append(id).Append('"');

                    if (--count != 0)
                        sb.Append(',');
                }
            }

            if (_uris != null)
            {
                foreach (string uri in _uris)
                {
                    sb.Append("uri=\"").Append(uri).Append('"');

                    if (--count != 0)
                        sb.Append(',');
                }
            }

            return sb.ToString();
        }

        public string HeaderName => null;

        /// <summary>
        /// Add an email to the ACL. Note that email support is only in these AWS regions:
        ///   <list type="bullet">
        ///     <item><term>US East (N. Virginia)</term></item>
        ///     <item><term>US West (N. California)</term></item>
        ///     <item><term>US West (Oregon)</term></item>
        ///     <item><term>Asia Pacific (Singapore)</term></item>
        ///     <item><term>Asia Pacific (Sydney)</term></item>
        ///     <item><term>Asia Pacific (Tokyo)</term></item>
        ///     <item><term>EU (Ireland)</term></item>
        ///     <item><term>South America (São Paulo)</term></item>
        /// </list>
        /// </summary>
        /// <param name="email">The email you want to add</param>
        public AclBuilder AddEmail(string email)
        {
            if (_emails == null)
                _emails = new HashSet<string>(/*1*/);

            if (!_emails.Add(email))
                throw new ArgumentException($"You already added the email {email}");

            return this;
        }

        /// <summary>
        /// Add a user id to the ACL. See https://docs.aws.amazon.com/general/latest/gr/acct-identifiers.html#FindingCanonicalId on how you find the
        /// user id associated with a user.
        /// </summary>
        /// <param name="id">The user id, for example 79a59df900b949e55d96a1e698fbacedfd6e09d98eacf8f8d5218e7cd47ef2be</param>
        public AclBuilder AddUserId(string id)
        {
            Validator.RequireNotNull(id);

            if (_ids == null)
                _ids = new HashSet<string>( /*1*/);

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
            if (_uris == null)
                _uris = new HashSet<string>( /*1*/);

            if (!_uris.Add(uri))
                throw new ArgumentException($"You already added the URI {uri}");

            return this;
        }

        /// <summary>Grant permission to a predefined Amazon S3 group.</summary>
        /// <param name="group">One of Amazon's predefined groups</param>
        public AclBuilder AddGroup(PredefinedGroup group)
        {
            return AddGroup(group.AsString(EnumFormat.EnumMemberValue));
        }
    }
}