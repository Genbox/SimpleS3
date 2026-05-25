using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Internals.Pools;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ObjectPoolTests
{
    [Fact]
    public void RentAndUseReturnsObjectWhenActionThrows()
    {
        ObjectPool<TestPooledObject> pool = new ObjectPool<TestPooledObject>();

        Assert.Throws<InvalidOperationException>(() => pool.RentAndUse<int>(_ => {}, _ => throw new InvalidOperationException()));

        TestPooledObject rented = pool.Rent(_ => {});

        Assert.Equal(1, rented.ResetCount);
    }

    [Fact]
    public async Task RentAndUseAsyncReturnsObjectWhenActionThrows()
    {
        ObjectPool<TestPooledObject> pool = new ObjectPool<TestPooledObject>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => pool.RentAndUseAsync<int>(_ => {}, _ => throw new InvalidOperationException()));

        TestPooledObject rented = pool.Rent(_ => {});

        Assert.Equal(1, rented.ResetCount);
    }

    [Fact]
    public void ReturnResetsObjectWhenPoolIsOverCapacity()
    {
        ObjectPool<TestPooledObject> pool = new ObjectPool<TestPooledObject>(0);
        TestPooledObject first = new TestPooledObject();
        TestPooledObject second = new TestPooledObject();

        pool.Return(first);
        pool.Return(second);

        Assert.Equal(1, second.ResetCount);
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    private sealed class TestPooledObject : IPooledObject
    {
        public int ResetCount { get; private set; }

        public void Reset() => ResetCount++;
    }
}