using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using MediatR;
using DispatchR;

namespace Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
[ThreadingDiagnoser]
public class MediatRVsDispatchRBenchmark
{
    private MediatR.IMediator _mediator;
    private DispatchR.IMediator _dispatcher;
    private ServiceProvider _serviceProvider;
    
    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblies(typeof(PingHandlerMediatR).Assembly));
        services.AddDispatchR(typeof(PingDispatchR).Assembly);

        _serviceProvider = services.BuildServiceProvider();
        _mediator = _serviceProvider.GetRequiredService<MediatR.IMediator>();
        _dispatcher = _serviceProvider.GetRequiredService<DispatchR.IMediator>();
    }

    [Benchmark]
    public async Task SendPing_With_MediatR_Parallel_100_000()
    {
        await Parallel.ForAsync(1, 100_000, new ParallelOptions(),
            async (index, ct) => { _ = await _mediator.Send(new PingMediatR(), CancellationToken.None); });
    }

    [Benchmark(Baseline = true)]
    public async Task SendPing_With_MediatR()
    {
        _ = await _mediator.Send(new PingMediatR(), CancellationToken.None);
    }

    [Benchmark]
    public async Task SendPing_With_DispatchR_Parallel_100_000()
    {
        await Parallel.ForAsync(1, 100_000, new ParallelOptions(),
            async (index, ct) => { _ = await _dispatcher.Send(new PingDispatchR(), CancellationToken.None); });
    }

    [Benchmark]
    public async Task SendPing_With_DispatchR()
    {
        _ = await _dispatcher.Send(new PingDispatchR(), CancellationToken.None);
    }
}