using System.Runtime.CompilerServices;
using System.Text;

namespace Genbox.SimpleS3.Core.Common.Pools;

public sealed class StringBuilderPool
{
    /// <summary>The default initial capacity of builder.</summary>
    private const int _defaultInitialBuilderCapacity = 100;

    /// <summary>The default maximum capacity of builder.</summary>
    private const int _defaultMaxBuilderCapacity = 8 * 1024;

    /// <summary>Number of builders per processor.</summary>
    private const int _builderCountPerProcessor = 5;

    /// <summary>The lazily-initialized shared pool instance.</summary>
    private static StringBuilderPool? _sharedInstance;

    /// <summary>Array of the retained builders.</summary>
    private readonly StringBuilder?[] _builders;

    /// <summary>Initial capacity of builder.</summary>
    private readonly int _initialBuilderCapacity;

    /// <summary>Maximum capacity of builder.</summary>
    private readonly int _maxBuilderCapacity;

    /// <summary>First builder.</summary>
    /// <remarks>The first builder is stored in a dedicated field, because we expect to be able to satisfy most requests from
    /// it.</remarks>
    private StringBuilder? _firstBuilder;

    /// <summary>Constructs an instance of the default pool of string builders using the default configuration settings.</summary>
    internal StringBuilderPool()
        : this(_defaultInitialBuilderCapacity, _defaultMaxBuilderCapacity, Environment.ProcessorCount * _builderCountPerProcessor) {}

    /// <summary>Constructs an instance of the default pool of string builders using the specified pool size.</summary>
    /// <param name="poolSize">Maximum number of builders stored in the pool.</param>
    internal StringBuilderPool(int poolSize)
        : this(_defaultInitialBuilderCapacity, _defaultMaxBuilderCapacity, poolSize) {}

    /// <summary>Constructs an instance of the default pool of string builders using the specified capacity settings.</summary>
    /// <param name="initialBuilderCapacity">Initial capacity of builder.</param>
    /// <param name="maxBuilderCapacity">Maximum capacity of builder.</param>
    internal StringBuilderPool(int initialBuilderCapacity, int maxBuilderCapacity)
        : this(initialBuilderCapacity, maxBuilderCapacity, Environment.ProcessorCount * _builderCountPerProcessor) {}

    /// <summary>Constructs an instance of the default pool of string builders using the specified capacity settings and pool
    /// size.</summary>
    /// <param name="initialBuilderCapacity">Initial capacity of builder.</param>
    /// <param name="maxBuilderCapacity">Maximum capacity of builder.</param>
    /// <param name="poolSize">Maximum number of builders stored in the pool.</param>
    internal StringBuilderPool(int initialBuilderCapacity, int maxBuilderCapacity, int poolSize)
    {
        if (initialBuilderCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(initialBuilderCapacity));

        if (maxBuilderCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxBuilderCapacity));

        if (poolSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(poolSize));

        if (initialBuilderCapacity > maxBuilderCapacity)
            throw new ArgumentOutOfRangeException(nameof(initialBuilderCapacity));

        _initialBuilderCapacity = initialBuilderCapacity;
        _maxBuilderCapacity = maxBuilderCapacity;
        _builders = new StringBuilder[poolSize - 1];
    }

    /// <summary>Retrieves a shared <see cref="StringBuilderPool" /> instance.</summary>
    public static StringBuilderPool Shared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Volatile.Read(ref _sharedInstance) ?? EnsureSharedCreated();
    }

    /// <summary>Ensures that <see cref="_sharedInstance" /> has been initialized to a pool and returns it.</summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static StringBuilderPool EnsureSharedCreated()
    {
        Interlocked.CompareExchange(ref _sharedInstance, new StringBuilderPool(), null);
        return _sharedInstance;
    }

    public StringBuilder Rent()
    {
        // Examine the first builder.
        // If that fails, then `RentViaScan` method will look at the remaining builders.
        StringBuilder? builder = _firstBuilder;
        if (builder == null || builder != Interlocked.CompareExchange(ref _firstBuilder, null, builder))
            builder = RentViaScan();

        return builder;
    }

    public StringBuilder Rent(int capacity)
    {
        if (capacity <= _maxBuilderCapacity)
            return Rent();

        return new StringBuilder(capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private StringBuilder RentViaScan()
    {
        StringBuilder?[] builders = _builders;
        int builderCount = builders.Length;

        for (int builderIndex = 0; builderIndex < builderCount; builderIndex++)
        {
            StringBuilder? builder = builders[builderIndex];
            if (builder != null && builder == Interlocked.CompareExchange(ref builders[builderIndex], null, builder))
                return builder;
        }

        return new StringBuilder(_initialBuilderCapacity);
    }

    public void Return(StringBuilder builder)
    {
        if (builder.Capacity > _maxBuilderCapacity)
            return;

        if (_firstBuilder == null)
        {
            builder.Clear();
            _firstBuilder = builder;
        }
        else
            ReturnViaScan(builder);
    }

    public string ReturnString(StringBuilder builder)
    {
        string value = builder.ToString();
        Return(builder);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReturnViaScan(StringBuilder builder)
    {
        StringBuilder?[] builders = _builders;
        int builderCount = builders.Length;

        for (int builderIndex = 0; builderIndex < builderCount; builderIndex++)
        {
            if (builders[builderIndex] == null)
            {
                builder.Clear();
                builders[builderIndex] = builder;
                break;
            }
        }
    }
}