using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using MediatR;
using DispatchR;
using IMediator = MediatR.IMediator;

namespace Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class MediatRVsDispatchWithPipelineRBenchmark
{
    public const int TotalSendRequests = 50_000;
    private ServiceProvider _serviceProvider;
    public List<IServiceScope> Scopes { get; set; } = new(TotalSendRequests);

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(PingHandlerMediatR).Assembly));
        services.AddTransient<IPipelineBehavior<PingMediatR, string>, LoggingBehaviorMediat>();
        services.AddDispatchR(typeof(PingDispatchR).Assembly);

        _serviceProvider = services.BuildServiceProvider();
        Parallel.For(0, TotalSendRequests, i => { Scopes.Add(_serviceProvider.CreateScope()); });
    }

    [Benchmark]
    public async Task<string> MediatR___SendPing_WithPipeline_ScopePerRequest_Parallel()
    {
        var res = string.Empty;
        await Parallel.ForEachAsync(Scopes,
            async (scope, ct) =>
            {
                // using (scope)
                {
                    res = await scope.ServiceProvider.GetRequiredService<MediatR.IMediator>()
                        .Send(new PingMediatR(), CancellationToken.None);
                }
            });

        return res;
    }

    [Benchmark]
    public async Task<string> MediatR___SendPing_WithPipeline_DefaultScope_Parallel()
    {
        var res = string.Empty;
        await Parallel.ForAsync(1, TotalSendRequests,
            async (index, ct) =>
            {
                res = await _serviceProvider.GetRequiredService<MediatR.IMediator>()
                    .Send(new PingMediatR(), CancellationToken.None);
            });

        return res;
    }

    [Benchmark(Baseline = true)]
    public Task<string> MediatR___SendPing_WithPipeline_NoScope_Single()
    {
        return _serviceProvider.GetRequiredService<MediatR.IMediator>()
            .Send(new PingMediatR(), CancellationToken.None);
    }
    
    public Task<string> MediatR___SendWitOutHandler_NoScope_Single()
    {
        try
        {
            return _serviceProvider.GetRequiredService<IMediator>()
                .Send(new PingMediatRWithOutHandler(), CancellationToken.None);
        }
        catch
        {
            return Task.FromResult(string.Empty);
        }
    }

    [Benchmark]
    public async Task<string> DispatchR_SendPing_WithPipeline_ScopePerRequest_Parallel()
    {
        var res = string.Empty;
        await Parallel.ForEachAsync(Scopes,
            async (scope, ct) =>
            {
                // using (scope)
                {
                    res = await scope.ServiceProvider.GetRequiredService<DispatchR.IMediator>()
                        .Send(new PingDispatchR(), CancellationToken.None);
                }
            });

        return res;
    }

    [Benchmark]
    public async Task<string> DispatchR_SendPing_WithPipeline_DefaultScope_Parallel()
    {
        var res = string.Empty;
        await Parallel.ForAsync(1, TotalSendRequests,
            async (index, ct) =>
            {
                res = await _serviceProvider.GetRequiredService<DispatchR.IMediator>()
                    .Send(new PingDispatchR(), CancellationToken.None);
            });

        return res;
    }

    [Benchmark]
    public Task<string> DispatchR_SendPing_WithPipeline_NoScope_Single()
    {
        return _serviceProvider.GetRequiredService<DispatchR.IMediator>()
            .Send(new PingDispatchR(), CancellationToken.None);
    }
    
    [Benchmark]
    public Task<string> DispatchR_SendWitOutHandler_NoScope_Single()
    {
        try
        {
            return _serviceProvider.GetRequiredService<DispatchR.IMediator>()
                .Send(new PingDispatchRWithOutHandler(), CancellationToken.None);
        }
        catch
        {
            return Task.FromResult(string.Empty);
        }
    }
}