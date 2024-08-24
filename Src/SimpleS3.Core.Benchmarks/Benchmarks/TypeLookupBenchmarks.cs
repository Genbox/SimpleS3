namespace Genbox.SimpleS3.Core.Benchmarks.Benchmarks;

public class TypeLookupBenchmarks
{
    private readonly HashSet<string> _stringDict = new HashSet<string>();
    private readonly Type _type = typeof(TypeLookupBenchmarks);
    private readonly HashSet<Type> _typeDict = new HashSet<Type>();

    [IterationCleanup]
    public void Cleanup()
    {
        _stringDict.Clear();
        _typeDict.Clear();
    }

    [Benchmark]
    public void AddWithString()
    {
        _stringDict.Add(_type.Name);
    }

    [Benchmark]
    public bool LookupWithString() => _stringDict.Contains(_type.Name);

    [Benchmark]
    public void AddWithType()
    {
        _typeDict.Add(_type);
    }

    [Benchmark]
    public bool LookupWithType() => _typeDict.Contains(_type);
}