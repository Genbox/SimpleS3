using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Genbox.SimpleS3.Core.Benchmarks.ClrTests
{
    [InProcess]
    public class GenericsBenchmarks
    {
        public interface IRequest
        {
            bool DoWork();
        }

        public class TestRequest : IRequest
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public bool DoWork()
            {
                //Don't do anything
                return true;
            }
        }

        public static class Helper
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static bool TrySomething(IRequest request)
            {
                return request.DoWork();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public static bool TrySomethingGeneric<T>(T request) where T : IRequest
            {
                return request.DoWork();
            }
        }

        private static readonly IRequest _request = new TestRequest();

        [Benchmark]
        public bool Interface()
        {
            return Helper.TrySomething(_request);
        }

        [Benchmark]
        public bool Generic()
        {
            return Helper.TrySomethingGeneric(_request);
        }
    }
}