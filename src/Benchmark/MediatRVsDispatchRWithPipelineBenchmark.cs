using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using MediatR;
using DispatchR;
using Mediator;
using IMediator = MediatR.IMediator;

namespace Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class MediatRVsDispatchWithPipelineRBenchmark
{
    private const int TotalSendRequests = 5_000;
    private IServiceScope _serviceScopeForMediatRWithPipeline;
    private IServiceScope _serviceScopeForMediatSgWithPipeline;
    private IServiceScope _serviceScopeForDispatchRWithPipeline;
    private DispatchR.IMediator _dispatchRWithPipeline;
    private IMediator _mediatRWithPipeline;
    private Mediator.IMediator _mediatSgWithPipeline;
    private static readonly PingDispatchR StaticDispatchR = new();
    private static readonly PingMediatR StaticPingMediatR = new();
    private static readonly PingMediatSG StaticPingMediatSg = new();
    private static readonly PingDispatchRWithOutHandler StaticDispatchRRequestWithOutHandler = new();
    private static readonly PingMediatRWithOutHandler StaticPingMediatRWithOutHandler = new();
    private static readonly PingMediatSGWithOutHandler StaticPingMediatSgWithOutHandler = new();
    private static List<IServiceScope> ScopesForMediatRWithPipeline { get; set; } = new(TotalSendRequests);
    private static List<IServiceScope> ScopesForMediatSgWithPipeline { get; set; } = new(TotalSendRequests);
    private static List<IServiceScope> ScopesForDispatchRWithPipeline { get; set; } = new(TotalSendRequests);

    [GlobalSetup]
    public void Setup()
    {
        var withPipelineServices = new ServiceCollection();
        
        withPipelineServices.AddMediatR(cfg =>
        {
            cfg.Lifetime = ServiceLifetime.Scoped;
            cfg.RegisterServicesFromAssemblies(typeof(PingHandlerMediatR).Assembly);
        });
        withPipelineServices.AddScoped<MediatR.IPipelineBehavior<PingMediatR, int>, LoggingBehaviorMediat>();

        withPipelineServices.AddMediator((MediatorOptions options) =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            // options.PipelineBehaviors = [typeof(LoggingBehaviorMediatSG)];
        });
        withPipelineServices.AddScoped<Mediator.IPipelineBehavior<PingMediatSG, int>, LoggingBehaviorMediatSG>();

        withPipelineServices.AddDispatchR(typeof(PingDispatchR).Assembly);
        var buildServicesWithoutPipeline = withPipelineServices.BuildServiceProvider();
        _dispatchRWithPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<DispatchR.IMediator>();
        _mediatRWithPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<MediatR.IMediator>();
        _mediatSgWithPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<Mediator.IMediator>();
        _serviceScopeForMediatRWithPipeline = buildServicesWithoutPipeline.CreateScope();
        _serviceScopeForMediatSgWithPipeline = buildServicesWithoutPipeline.CreateScope();
        _serviceScopeForDispatchRWithPipeline = buildServicesWithoutPipeline.CreateScope();
        ScopesForMediatRWithPipeline.Clear();
        ScopesForMediatSgWithPipeline.Clear();
        ScopesForDispatchRWithPipeline.Clear();
        Parallel.For(0, TotalSendRequests, i =>
        {
            ScopesForMediatRWithPipeline.Add(buildServicesWithoutPipeline.CreateScope());
            ScopesForDispatchRWithPipeline.Add(buildServicesWithoutPipeline.CreateScope());
            ScopesForMediatSgWithPipeline.Add(buildServicesWithoutPipeline.CreateScope());
        });
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceScopeForMediatRWithPipeline.Dispose();
        _serviceScopeForMediatSgWithPipeline.Dispose();
        _serviceScopeForDispatchRWithPipeline.Dispose();
        _dispatchRWithPipeline = null!;
        _mediatRWithPipeline = null!;
        _mediatSgWithPipeline = null!;
        Parallel.ForEach(ScopesForMediatRWithPipeline, scope => scope.Dispose());
        Parallel.ForEach(ScopesForMediatSgWithPipeline, scope => scope.Dispose());
        Parallel.ForEach(ScopesForDispatchRWithPipeline, scope => scope.Dispose());
    }
    
    #region SendRequest_With_Pipeline_ExistCommand_ExistMediator_WithOutHandler

    [Benchmark(Baseline = true)]
    public Task<int> MediatR___SendRequest_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _mediatRWithPipeline.Send(StaticPingMediatRWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return Task.FromResult(0);
        }
    }

    [Benchmark]
    public ValueTask<int> MediatSG__SendRequest_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _mediatSgWithPipeline.Send(StaticPingMediatSgWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return ValueTask.FromResult(0);
        }
    }
    
    [Benchmark]
    public ValueTask<int> DispatchR_SendRequest_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _dispatchRWithPipeline.Send(StaticDispatchRRequestWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return ValueTask.FromResult(0);
        }
    }

    #endregion
    
    #region SendRequest_ExistCommand_ExistMediator

    [Benchmark]
    public Task<int> MediatR___SendRequest_ExistCommand_ExistMediator()
    {
        return _mediatRWithPipeline.Send(StaticPingMediatR, CancellationToken.None);
    }
    
    [Benchmark]
    public ValueTask<int> MediatSG__SendRequest_ExistCommand_ExistMediator()
    {
        return _mediatSgWithPipeline.Send(StaticPingMediatSg, CancellationToken.None);
    }

    [Benchmark]
    public ValueTask<int> DispatchR_SendRequest_ExistCommand_ExistMediator()
    {
        return _dispatchRWithPipeline.Send(StaticDispatchR, CancellationToken.None);
    }

    #endregion
    
    #region SendRequest_ExistCommand_GetMediator

    [Benchmark]
    public Task<int> MediatR___SendRequest_ExistCommand_GetMediator()
    {
        return _serviceScopeForMediatRWithPipeline
            .ServiceProvider
            .GetRequiredService<MediatR.IMediator>()
            .Send(StaticPingMediatR, CancellationToken.None);
    }
    
    [Benchmark]
    public ValueTask<int> MediatSG__SendRequest_ExistCommand_GetMediator()
    {
        return _serviceScopeForMediatSgWithPipeline
            .ServiceProvider
            .GetRequiredService<Mediator.IMediator>()
            .Send(StaticPingMediatSg, CancellationToken.None);
    }

    [Benchmark]
    public ValueTask<int> DispatchR_SendRequest_ExistCommand_GetMediator()
    {
        return _serviceScopeForDispatchRWithPipeline
            .ServiceProvider
            .GetRequiredService<DispatchR.IMediator>()
            .Send(StaticDispatchR, CancellationToken.None);
    }

    #endregion
    
    #region SendRequest_ExistCommand_ExistMediator_Parallel
    
    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> MediatR___SendRequest_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _mediatRWithPipeline.Send(StaticPingMediatR, CancellationToken.None);
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> MediatSG__SendRequest_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _mediatSgWithPipeline.Send(StaticPingMediatSg, CancellationToken.None);
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> DispatchR_SendRequest_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _dispatchRWithPipeline.Send(StaticDispatchR, CancellationToken.None);
        });
        
        return result;
    }
    
    #endregion
    
    #region SendRequest_ExistCommand_GetMediator_ExistScopes_Parallel
    
    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> MediatR___SendRequest_ExistCommand_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatRWithPipeline, async (scope, ct) =>
        {
            result = await scope.ServiceProvider.GetRequiredService<MediatR.IMediator>()
                .Send(StaticPingMediatR, CancellationToken.None);
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> MediatSG__SendRequest_ExistCommand_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatSgWithPipeline, async (scope, ct) =>
        {
            result = await scope.ServiceProvider.GetRequiredService<Mediator.IMediator>()
                .Send(StaticPingMediatSg, CancellationToken.None);
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalSendRequests)]
    public async Task<int> DispatchR_SendRequest_ExistCommand_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForDispatchRWithPipeline, async (scope, ct) =>
        {
            result = await scope.ServiceProvider.GetRequiredService<DispatchR.IMediator>()
                .Send(StaticDispatchR, CancellationToken.None);
        });
        
        return result;
    }
    
    #endregion
}