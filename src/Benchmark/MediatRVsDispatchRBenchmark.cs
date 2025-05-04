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
public class MediatRVsDispatchBenchmark
{
    private const int TotalSendRequests = 5_000;
    private IServiceScope _serviceScopeForMediatRWithoutPipeline;
    private IServiceScope _serviceScopeForMediatSgWithoutPipeline;
    private IServiceScope _serviceScopeForDispatchRWithoutPipeline;
    private DispatchR.IMediator _dispatchRWithoutPipeline;
    private IMediator _mediatRWithoutPipeline;
    private Mediator.IMediator _mediatSgWithoutPipeline;
    private static readonly PingDispatchR StaticDispatchR = new();
    private static readonly PingMediatR StaticPingMediatR = new();
    private static readonly PingMediatSG StaticPingMediatSg = new();
    private static readonly PingDispatchRWithOutHandler StaticDispatchRRequestWithOutHandler = new();
    private static readonly PingMediatRWithOutHandler StaticPingMediatRWithOutHandler = new();
    private static readonly PingMediatSGWithOutHandler StaticPingMediatSgWithOutHandler = new();
    private static List<IServiceScope> ScopesForMediatRWithoutPipeline { get; set; } = new(TotalSendRequests);
    private static List<IServiceScope> ScopesForMediatSgWithoutPipeline { get; set; } = new(TotalSendRequests);
    private static List<IServiceScope> ScopesForDispatchRWithoutPipeline { get; set; } = new(TotalSendRequests);

    [GlobalSetup]
    public void Setup()
    {
        var withoutPipelineServices = new ServiceCollection();
        withoutPipelineServices.AddMediatR(cfg =>
        {
            cfg.Lifetime = ServiceLifetime.Scoped;
            cfg.RegisterServicesFromAssemblies(typeof(PingHandlerMediatR).Assembly);
        });
        withoutPipelineServices.AddMediator(opts =>
        {
            opts.ServiceLifetime = ServiceLifetime.Scoped;
        });
        withoutPipelineServices.AddDispatchR(typeof(PingDispatchR).Assembly, withPipelines: false);
        var buildServicesWithoutPipeline = withoutPipelineServices.BuildServiceProvider();
        _dispatchRWithoutPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<DispatchR.IMediator>();
        _mediatRWithoutPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<MediatR.IMediator>();
        _mediatSgWithoutPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<Mediator.IMediator>();
        _serviceScopeForMediatRWithoutPipeline = buildServicesWithoutPipeline.CreateScope();
        _serviceScopeForMediatSgWithoutPipeline = buildServicesWithoutPipeline.CreateScope();
        _serviceScopeForDispatchRWithoutPipeline = buildServicesWithoutPipeline.CreateScope();
        ScopesForMediatRWithoutPipeline.Clear();
        ScopesForMediatSgWithoutPipeline.Clear();
        ScopesForDispatchRWithoutPipeline.Clear();
        Parallel.For(0, TotalSendRequests, i =>
        {
            ScopesForMediatRWithoutPipeline.Add(buildServicesWithoutPipeline.CreateScope());
            ScopesForDispatchRWithoutPipeline.Add(buildServicesWithoutPipeline.CreateScope());
            ScopesForMediatSgWithoutPipeline.Add(buildServicesWithoutPipeline.CreateScope());
        });
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceScopeForMediatRWithoutPipeline.Dispose();
        _serviceScopeForMediatSgWithoutPipeline.Dispose();
        _serviceScopeForDispatchRWithoutPipeline.Dispose();
        _dispatchRWithoutPipeline = null!;
        _mediatRWithoutPipeline = null!;
        _mediatSgWithoutPipeline = null!;
        Parallel.ForEach(ScopesForMediatRWithoutPipeline, scope => scope.Dispose());
        Parallel.ForEach(ScopesForMediatSgWithoutPipeline, scope => scope.Dispose());
        Parallel.ForEach(ScopesForDispatchRWithoutPipeline, scope => scope.Dispose());
    }
    
    #region SendRequest_With_ExistCommand_ExistMediator_WithOutHandler

    [Benchmark(Baseline = true)]
    public Task<int> MediatR___SendRequest_With_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _mediatRWithoutPipeline.Send(StaticPingMediatRWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return Task.FromResult(0);
        }
    }
    
    [Benchmark]
    public ValueTask<int> MediatSG__SendRequest_With_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _mediatSgWithoutPipeline.Send(StaticPingMediatSgWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return ValueTask.FromResult(0);
        }
    }

    [Benchmark]
    public ValueTask<int> DispatchR_SendRequest_With_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _dispatchRWithoutPipeline.Send(StaticDispatchRRequestWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return ValueTask.FromResult(0);
        }
    }

    #endregion

    #region SendRequest_With_ExistCommand_ExistMediator

    [Benchmark]
    public Task<int> MediatR___SendRequest_With_ExistCommand_ExistMediator()
    {
        return _mediatRWithoutPipeline.Send(StaticPingMediatR, CancellationToken.None);
    }
    
    [Benchmark]
    public ValueTask<int> MediatSG__SendRequest_With_ExistCommand_ExistMediator()
    {
        return _mediatSgWithoutPipeline.Send(StaticPingMediatSg, CancellationToken.None);
    }

    [Benchmark]
    public ValueTask<int> DispatchR_SendRequest_With_ExistCommand_ExistMediator()
    {
        return _dispatchRWithoutPipeline.Send(StaticDispatchR, CancellationToken.None);
    }

    #endregion
    
    #region SendRequest_With_ExistCommand_GetMediator

    [Benchmark]
    public async Task<int> MediatR___SendRequest_With_ExistCommand_GetMediator()
    {
        var result = await _serviceScopeForMediatRWithoutPipeline
            .ServiceProvider
            .GetRequiredService<MediatR.IMediator>()
            .Send(StaticPingMediatR, CancellationToken.None);
        
        return result;
    }
    
    [Benchmark]
    public async ValueTask<int> MediatSG__SendRequest_With_ExistCommand_GetMediator()
    {
        var result = await _serviceScopeForMediatSgWithoutPipeline
            .ServiceProvider
            .GetRequiredService<Mediator.IMediator>()
            .Send(StaticPingMediatSg, CancellationToken.None);
        
        return result;
    }

    [Benchmark]
    public ValueTask<int> DispatchR_SendRequest_With_ExistCommand_GetMediator()
    {
        return _serviceScopeForDispatchRWithoutPipeline
            .ServiceProvider
            .GetRequiredService<DispatchR.IMediator>()
            .Send(StaticDispatchR, CancellationToken.None);
    }

    #endregion
    
    #region SendRequest_With_ExistCommand_ExistMediator_Parallel

    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> MediatR___SendRequest_With_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _mediatRWithoutPipeline.Send(StaticPingMediatR, CancellationToken.None);
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> MediatSG__SendRequest_With_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _mediatSgWithoutPipeline.Send(StaticPingMediatSg, CancellationToken.None);
        });
        
        return result;
    }

    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> DispatchR_SendRequest_With_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _dispatchRWithoutPipeline.Send(StaticDispatchR, CancellationToken.None);
        });
        
        return result;
    }

    #endregion
    
    #region SendRequest_With_ExistCommand_GetMediator_ExistScopes_Parallel

    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> MediatR___SendRequest_With_ExistCommand_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatRWithoutPipeline, async (scope, ct) =>
        {
            result = await scope.ServiceProvider.GetRequiredService<MediatR.IMediator>()
                .Send(StaticPingMediatR, CancellationToken.None);
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> MediatSG__SendRequest_With_ExistCommand_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatSgWithoutPipeline, async (scope, ct) =>
        {
            result = await scope.ServiceProvider.GetRequiredService<Mediator.IMediator>()
                .Send(StaticPingMediatSg, CancellationToken.None);
        });
        
        return result;
    }

    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> DispatchR_SendRequest_With_ExistCommand_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForDispatchRWithoutPipeline, async (scope, ct) =>
        {
            result = await scope.ServiceProvider.GetRequiredService<DispatchR.IMediator>()
                .Send(StaticDispatchR, CancellationToken.None);
        });
        
        return result;
    }

    #endregion
}