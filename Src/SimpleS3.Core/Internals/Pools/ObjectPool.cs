using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Genbox.SimpleS3.Core.Internals.Pools;

internal class ObjectPool<T>(int maxCapacity = 32) where T : IPooledObject
{
    public static readonly ObjectPool<T> Shared = new ObjectPool<T>();
    private readonly ConcurrentBag<T> _pool = new ConcurrentBag<T>();
    private int _count;

    public int Count => Volatile.Read(ref _count);

    [SuppressMessage("Major Code Smell", "S3011", Justification = "Pooled implementation types may intentionally expose non-public parameterless constructors.")]
    public T Rent(Action<T> setup)
    {
        if (!_pool.TryTake(out T? obj))
            obj = (T)Activator.CreateInstance(typeof(T), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, CultureInfo.InvariantCulture)!;

        else
            Interlocked.Decrement(ref _count);

        try
        {
            setup(obj);
            return obj;
        }
        catch
        {
            Return(obj);
            throw;
        }
    }

    public async Task<TReturn> RentAndUseAsync<TReturn>(Action<T> setup, Func<T, Task<TReturn>> action)
    {
        T obj = Rent(setup);

        try
        {
            return await action(obj);
        }
        finally
        {
            Return(obj);
        }
    }

    public void Return(T obj)
    {
        //We reset here instead of in Rent() because it frees memory for strings and objects by releasing references.
        obj.Reset();

        if (Interlocked.Increment(ref _count) > maxCapacity)
        {
            Interlocked.Decrement(ref _count);
            return;
        }

        _pool.Add(obj);
    }

    public void Return(IEnumerable<T> objects)
    {
        foreach (T obj in objects)
            Return(obj);
    }
}