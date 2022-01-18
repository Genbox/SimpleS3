using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[InProcess]
public class TypeLookupBenchmarks
{
    private readonly HashSet<string> _stringDict = new HashSet<string>();
    private readonly HashSet<Type> _typeDict = new HashSet<Type>();
    private readonly Type _type = typeof(TypeLookupBenchmarks);

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
    public bool LookupWithString()
    {
        return _stringDict.Contains(_type.Name);
    }

    [Benchmark]
    public void AddWithType()
    {
        _typeDict.Add(_type);
    }

    [Benchmark]
    public bool LookupWithType()
    {
        return _typeDict.Contains(_type);
    }
}