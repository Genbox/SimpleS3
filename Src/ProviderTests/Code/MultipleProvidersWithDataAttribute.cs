using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit.Sdk;
using Xunit.v3;

namespace Genbox.ProviderTests.Code;

internal sealed class MultipleProvidersWithDataAttribute(S3Provider providers, params object[] otherData) : DataAttribute
{
    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        TheoryData<S3Provider, string, ISimpleClient, object> data = new TheoryData<S3Provider, string, ISimpleClient, object>();

        foreach ((S3Provider provider, IProfile profile, ISimpleClient client) in ProviderSetup.Instance.Clients)
        {
            string bucket = UtilityHelper.GetTestBucket(profile);

            foreach (object o in otherData)
            {
                TheoryDataRow<S3Provider, string, ISimpleClient, object> row = new TheoryDataRow<S3Provider, string, ISimpleClient, object>(provider, bucket, client, o);

                if (!providers.HasFlag(provider))
                    row.Skip = "Not supported";

                data.Add(row);
            }
        }

        return ValueTask.FromResult<IReadOnlyCollection<ITheoryDataRow>>(data);
    }

    public override bool SupportsDiscoveryEnumeration() => true;
}