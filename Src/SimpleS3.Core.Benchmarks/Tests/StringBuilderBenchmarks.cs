using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Common.Pools;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[InProcess]
public class StringBuilderBenchmarks
{
    private const string _equalSign = "=";
    private readonly StringBuilder _append = new StringBuilder();
    private readonly StringBuilder _appendFormat = new StringBuilder();

    [Benchmark]
    public string Append()
    {
        _append.Clear();
        return _append.Append("MyKey").Append('=').Append("MyValue").ToString();
    }

    [Benchmark]
    public string AppendFormat()
    {
        _appendFormat.Clear();
        return _appendFormat.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}", "MyKey", "MyValue").ToString();
    }

    [Benchmark]
    public string NormalAdd() => "MyKey" + _equalSign + "MyValue"; //We use a field here to avoid a smart compiler from creating a constant

    [Benchmark]
    public string Interpolation() => $"MyKey{_equalSign}MyValue"; //We use a field here to avoid a smart compiler from creating a constant

    [Benchmark]
    public string FromPool()
    {
        StringBuilder sb = StringBuilderPool.Shared.Rent();

        //We use a field here to avoid a smart compiler from creating a constant
        sb.Append("MyKey").Append(_equalSign).Append("MyValue");

        return StringBuilderPool.Shared.ReturnString(sb);
    }
}