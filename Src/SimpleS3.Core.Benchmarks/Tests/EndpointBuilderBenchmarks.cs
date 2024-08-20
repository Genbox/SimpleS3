using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[MemoryDiagnoser]
public class EndpointBuilderBenchmarks
{
    private EndpointBuilder _builder;
    private EndpointBuilder _builder2;
    private IRequest _request;

    [GlobalSetup]
    public void Setup()
    {
        SimpleS3Config config = new SimpleS3Config(null!, "eu-west-1");
        config.EndpointTemplate = "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com";

        SimpleS3Config config2 = new SimpleS3Config(null!, "eu-west-1");
        config2.Endpoint = new Uri("http://bucket.s3.eu-west-1.amazonaws.com");

        _builder = new EndpointBuilder(Options.Create(config));
        _builder2 = new EndpointBuilder(Options.Create(config2));

        _request = new GetObjectRequest("bucket", "object");
    }

    [Benchmark]
    public IEndpointData EndpointTemplate() => _builder.GetEndpoint(_request);

    [Benchmark]
    public IEndpointData Endpoint() => _builder2.GetEndpoint(_request);
}