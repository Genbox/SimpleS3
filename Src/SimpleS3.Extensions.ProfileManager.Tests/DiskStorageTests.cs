using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Extensions.ProfileManager.Storage;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Tests;

public sealed class DiskStorageTests : IDisposable
{
    private readonly string _profileLocation = Path.Combine(Path.GetTempPath(), "SimpleS3-" + Guid.NewGuid());

    [Theory]
    [InlineData("..")]
    [InlineData("../outside")]
    [InlineData("nested/profile")]
    public void PutRejectsUnsafeProfileNames(string name)
    {
        DiskStorage storage = CreateStorage();

        Assert.Throws<S3Exception>(() => storage.Put(name, [1, 2, 3]));
    }

    [Fact]
    public void ListReturnsProfileNames()
    {
        DiskStorage storage = CreateStorage();
        storage.Put("profile", [1, 2, 3]);

        Assert.Equal(["profile"], storage.List());
    }

    public void Dispose()
    {
        if (Directory.Exists(_profileLocation))
            Directory.Delete(_profileLocation, true);
    }

    private DiskStorage CreateStorage() => new DiskStorage(Options.Create(new DiskStorageOptions { ProfileLocation = _profileLocation }));
}