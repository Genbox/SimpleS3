using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class LifecycleTransitionTests
{
    [Fact]
    public void ZeroDayTransitionsAreAllowed()
    {
        S3Transition transition = new S3Transition(0, StorageClass.Standard);

        Assert.Equal(0, transition.TransitionAfterDays);
        Assert.Equal(StorageClass.Standard, transition.StorageClass);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void UnknownStorageClassesAreRejected(bool useDate)
    {
        Assert.Throws<RequireException>(() =>
        {
            _ = useDate
                ? new S3Transition(DateTimeOffset.UtcNow, StorageClass.Unknown)
                : new S3Transition(1, StorageClass.Unknown);
        });
    }
}