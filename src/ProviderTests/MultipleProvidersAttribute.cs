using System.Collections.Generic;
using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit.Sdk;

namespace Genbox.ProviderTests
{
    public sealed class MultipleProvidersAttribute : DataAttribute
    {
        private readonly object[] _otherData;
        private readonly S3Provider _providers;
        private bool _shouldSkip;

        public MultipleProvidersAttribute(S3Provider providers, params object[] otherData)
        {
            _providers = providers;
            _otherData = otherData;
        }

        public override string? Skip => _shouldSkip ? "Not supported" : null;

        public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
        {
            foreach ((S3Provider provider, IProfile profile, ISimpleClient client) in ProviderSetup.Instance.Clients)
            {
                _shouldSkip = !_providers.HasFlag(provider);

                if (_otherData.Length > 0)
                {
                    foreach (object o in _otherData)
                    {
                        yield return new[] { provider, profile, client, o };
                    }
                }
                else
                    yield return new object?[] { provider, profile, client };
            }
        }
    }
}