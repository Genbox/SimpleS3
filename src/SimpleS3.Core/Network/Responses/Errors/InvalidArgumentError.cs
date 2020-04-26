using System.Collections.Generic;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Errors
{
    [PublicAPI]
    public class InvalidArgumentError : GenericError
    {
        internal InvalidArgumentError(IDictionary<string, string> lookup) : base(lookup)
        {
            ArgumentName = lookup["ArgumentName"];
            ArgumentValue = lookup["ArgumentValue"];
        }

        public string ArgumentName { get; }
        public string ArgumentValue { get; }

        public override string GetErrorDetails()
        {
            return $"Argument: {ArgumentName} - Value: {ArgumentValue}";
        }
    }
}