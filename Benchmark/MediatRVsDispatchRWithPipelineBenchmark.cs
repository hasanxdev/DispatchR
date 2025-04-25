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
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class MediatRVsDispatchWithPipelineRBenchmark
{
    public const int TotalSendRequests = 100_000;
    private ServiceProvider _serviceProvider;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(PingHandlerMediatR).Assembly));
        services.AddDispatchR(typeof(PingDispatchR).Assembly);
        services.AddTransient<IPipelineBehavior<PingMediatR, string>, LoggingBehaviorMediat>();

        _serviceProvider = services.BuildServiceProvider();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceProvider.Dispose();
    }

    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<string> MediatR___SendPing_WithEmptyPipeline_ScopePerRequest_Parallel()
    {
        var res = string.Empty;
        await Parallel.ForAsync(1, TotalSendRequests, new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            },
            async (index, ct) =>
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                res = await scope.ServiceProvider.GetRequiredService<MediatR.IMediator>()
                    .Send(new PingMediatR(), CancellationToken.None);
            });
        
        return res;
    }

    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<string> MediatR___SendPing_WithEmptyPipeline_NoScope_Parallel()
    {
        var res = string.Empty;
        await Parallel.ForAsync(1, TotalSendRequests, new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            },
            async (index, ct) =>
            {
                res = await _serviceProvider.GetRequiredService<MediatR.IMediator>()
                    .Send(new PingMediatR(), CancellationToken.None);
            });
        
        return res;
    }

    [Benchmark(Baseline = true)]
    public async Task<string> MediatR___SendPing_WithEmptyPipeline_NoScope_Single()
    {
        return await _serviceProvider.GetRequiredService<MediatR.IMediator>()
            .Send(new PingMediatR(), CancellationToken.None);
    }

    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<string> DispatchR_SendPing_WithEmptyPipeline_ScopePerRequest_Parallel()
    {
        var res = string.Empty;
        await Parallel.ForAsync(1, TotalSendRequests, new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            },
            async (index, ct) =>
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                res = await scope.ServiceProvider.GetRequiredService<DispatchR.IMediator>()
                    .Send(new PingDispatchR(), CancellationToken.None);
            });
        
        return res;
    }

    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<string> DispatchR_SendPing_WithEmptyPipeline_NoScope_Parallel()
    {
        var res = string.Empty;
        await Parallel.ForAsync(1, TotalSendRequests, new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount * 2
            },
            async (index, ct) =>
            {
                res = await _serviceProvider.GetRequiredService<DispatchR.IMediator>()
                    .Send(new PingDispatchR(), CancellationToken.None);
            });
        
        return res;
    }

    [Benchmark]
    public async Task<string> DispatchR_SendPing_WithEmptyPipeline_NoScope_Single()
    {
        return await _serviceProvider.GetRequiredService<DispatchR.IMediator>()
            .Send(new PingDispatchR(), CancellationToken.None);
    }
}