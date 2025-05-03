using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

using DispatchR;

using MediatR;

using System.Reflection;
namespace Benchmark;
public class Ping : IRequest<string> { }

public class PingHandler : MediatR.IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken cancellationToken) =>
        Task.FromResult("Pong");
}

[MemoryDiagnoser]
[Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest)]
public class DispatchRServiceCollectionBenchmark
{
    private static Assembly _assembly;

    [GlobalSetup]
    public void Setup()
    {

        _assembly = Assembly.GetExecutingAssembly();


    }
    [Benchmark]
    public void MediatR()
    {
        var services = new ServiceCollection();
        services.AddMediatR(typeof(PingHandler).Assembly);
        var provider = services.BuildServiceProvider();
        MediatR.IMediator _mediatr = provider.GetRequiredService<MediatR.IMediator>();
    }

    [Benchmark]
    public void BenchmarkAddDispatchR()
    {
        var services = new ServiceCollection();
        services.AddDispatchR(_assembly);
    }
    [Benchmark]
    public void BenchmarkAddDispatchR_V1()
    {
        var services = new ServiceCollection();
        services.AddDispatchRV1(_assembly);
    }
    [Benchmark]
    public void BenchmarkAddDispatchR_V2()
    {
        var services = new ServiceCollection();
        services.AddDispatchRV2(_assembly);
    }
    [Benchmark]
    public void BenchmarkAddDispatchR_V3()
    {
        var services = new ServiceCollection();
        services.AddDispatchRV3(_assembly);
    }

}
