using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit.Sdk;

namespace Genbox.ProviderTests.Code;

internal sealed class MultipleProvidersAttribute(S3Provider providers, params object[] otherData) : DataAttribute
{
    private bool _shouldSkip;

    public override string? Skip => _shouldSkip ? "Not supported" : null;

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
        foreach ((S3Provider provider, IProfile profile, ISimpleClient client) in ProviderSetup.Instance.Clients)
        {
            _shouldSkip = !providers.HasFlag(provider);
            string bucket = UtilityHelper.GetTestBucket(profile);

            if (otherData.Length > 0)
            {
                foreach (object o in otherData)
                    yield return [provider, bucket, client, o];
            }
            else
                yield return [provider, bucket, client];
        }
    }
}