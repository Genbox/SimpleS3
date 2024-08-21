using System.Reflection;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Network.Requests;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class PoolTypeTests
{
    /// <summary>Tests is each of the BaseRequest types has an internal constructor with zero parameters.</summary>
    [Fact]
    public void EnsureInternalCtor()
    {
        List<string> failed = new List<string>();

        foreach (Type requestType in GetRequestTypes())
        {
            bool hasCorrect = false;

            //This is a special request that is only used internally
            if (requestType == typeof(SignedRequest))
                continue;

            ConstructorInfo[] constructorInfos = requestType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (ConstructorInfo info in constructorInfos)
            {
                if (info.IsPublic || info.IsPrivate || info.GetParameters().Length != 0)
                    continue;

                hasCorrect = true;
                break;
            }

            if (!hasCorrect)
                failed.Add(requestType.Name);
        }

        Assert.Empty(failed);
    }

    /// <summary>Tests if each type has an initialize method, which is needed to correctly initialize a new state when using pools</summary>
    [Fact]
    public void EnsureInitializeMethod()
    {
        List<string> failed = new List<string>();

        foreach (Type requestType in GetRequestTypes())
        {
            //This is a special request that is only used internally
            if (requestType == typeof(SignedRequest))
                continue;

            MethodInfo[] methodInfos = requestType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.Name == "Initialize").ToArray();

            if (methodInfos.Length == 0)
                failed.Add(requestType.Name);

            foreach (MethodInfo info in methodInfos)
            {
                if (info.IsPublic || info.IsPrivate)
                    failed.Add(requestType.Name);
            }
        }

        Assert.Empty(failed);
    }

    private IEnumerable<Type> GetRequestTypes()
    {
        Type baseType = typeof(BaseRequest);
        IEnumerable<Type> types = TypeHelper.GetTypesInheritedFrom(baseType);

        foreach (Type type in types)
        {
            if (type.IsAbstract)
                continue;

            //This type does not have any properties what so ever, so it is exempt from the check.
            if (type == typeof(ListBucketsRequest))
                continue;

            yield return type;
        }
    }
}