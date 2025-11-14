using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit.Sdk;
using Xunit.v3;

namespace Genbox.ProviderTests.Code;

/// <summary>
/// Used to perform an action against just one provider
/// </summary>
internal sealed class SingleProviderAttribute(S3Provider provider) : DataAttribute
{
    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        TheoryData<S3Provider, string, ISimpleClient> data = new TheoryData<S3Provider, string, ISimpleClient>();

        foreach ((S3Provider s3Provider, IProfile profile, ISimpleClient client) in ProviderSetup.Instance.Clients)
        {
            if (provider != s3Provider)
                continue;

            string bucket = UtilityHelper.GetTestBucket(profile);
            data.Add(provider, bucket, client);
        }

        return ValueTask.FromResult<IReadOnlyCollection<ITheoryDataRow>>(data);
    }

    public override bool SupportsDiscoveryEnumeration() => true;
}