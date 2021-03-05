using System.Collections.Generic;
using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors
{
    [PublicAPI]
    public class PreconditionFailedError : GenericError
    {
        internal PreconditionFailedError(IDictionary<string, string> lookup) : base(lookup)
        {
            Condition = lookup.GetRequiredValue("Condition");
        }

        public string Condition { get; }

        public override string GetErrorDetails()
        {
            return "Condition: " + Condition;
        }
    }
}