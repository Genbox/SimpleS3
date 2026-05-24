using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit.Sdk;
using Xunit.v3;

namespace Genbox.ProviderTests.Code;

/// <summary>
/// Intended for multiple providers. Providers not enabled will show up as skipped. Used to show providers that don't support a particular action.
/// </summary>
internal sealed class MultipleProvidersAttribute(S3Provider providers) : DataAttribute
{
    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        TheoryData<S3Provider, string, ISimpleClient> data = new TheoryData<S3Provider, string, ISimpleClient>();

        foreach ((S3Provider provider, IProfile profile, ISimpleClient client) in ProviderSetup.Instance.Clients)
        {
            string bucket = UtilityHelper.GetTestBucket(profile);
            TheoryDataRow<S3Provider, string, ISimpleClient> row = new TheoryDataRow<S3Provider, string, ISimpleClient>(provider, bucket, client);

            if (!providers.HasFlag(provider))
                row.Skip = "Not supported";

            data.Add(row);
        }

        return ValueTask.FromResult<IReadOnlyCollection<ITheoryDataRow>>(data);
    }

    public override bool SupportsDiscoveryEnumeration() => true;
}