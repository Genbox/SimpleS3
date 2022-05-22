using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

namespace Genbox.SimpleS3.Core.Internals.Pools;

internal class ObjectPool<T> where T : IPooledObject
{
    public static readonly ObjectPool<T> Shared = new ObjectPool<T>();
    private readonly int _maxCapacity;
    private readonly ConcurrentBag<T> _pool;

    public ObjectPool(int maxCapacity = 32)
    {
        _maxCapacity = maxCapacity;
        _pool = new ConcurrentBag<T>();
    }

    public int Count => _pool.Count;

    public T Rent(Action<T> setup)
    {
        if (!_pool.TryTake(out T? obj))
            obj = (T)Activator.CreateInstance(typeof(T), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, CultureInfo.InvariantCulture)!;

        setup(obj);
        return obj;
    }

    public TReturn RentAndUse<TReturn>(Action<T> setup, Func<T, TReturn> action)
    {
        T obj = Rent(setup);
        TReturn returnVal = action(obj);
        Return(obj);
        return returnVal;
    }

    public async Task<TReturn> RentAndUseAsync<TReturn>(Action<T> setup, Func<T, Task<TReturn>> action)
    {
        T obj = Rent(setup);
        TReturn returnVal = await action(obj).ConfigureAwait(false);
        Return(obj);
        return returnVal;
    }

    public void Return(T obj)
    {
        if (_pool.Count > _maxCapacity)
            return;

        //We reset here instead of in Rent() because it frees memory for strings and objects by releasing references.
        obj.Reset();
        _pool.Add(obj);
    }

    public void Return(IEnumerable<T> objs)
    {
        if (_pool.Count > _maxCapacity)
            return;

        //We reset here instead of in Rent() because it frees memory for strings and objects by releasing references.
        foreach (T obj in objs)
        {
            obj.Reset();
            _pool.Add(obj);
        }
    }
}