using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Genbox.SimpleS3.Core.Benchmarks.ClrTests;

[InProcess]
public class GenericsBenchmarks
{
    private static readonly IRequest _request = new TestRequest();

    [Benchmark]
    public bool Interface() => Helper.TrySomething(_request);

    [Benchmark]
    public bool Generic() => Helper.TrySomethingGeneric(_request);

    private interface IRequest
    {
        bool DoWork();
    }

    private class TestRequest : IRequest
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool DoWork() => true;
    }

    private static class Helper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool TrySomething(IRequest request) => request.DoWork();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool TrySomethingGeneric<T>(T request) where T : IRequest => request.DoWork();
    }
}