using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit.Sdk;

namespace Genbox.ProviderTests.Code;

/// <summary>
/// Used to perform an action against just one provider
/// </summary>
internal sealed class SingleProviderAttribute(S3Provider provider, params object[] otherData) : DataAttribute
{
    public override string? Skip => null;

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
        foreach ((S3Provider s3Provider, IProfile profile, ISimpleClient client) in ProviderSetup.Instance.Clients)
        {
            if (provider != s3Provider)
                continue;

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