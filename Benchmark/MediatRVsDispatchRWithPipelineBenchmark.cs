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
    private IServiceScope _serviceScopeForMediatR;
    private IServiceScope _serviceScopeForMediatRWithoutPipeline;
    private IServiceScope _serviceScopeForDispatchR;
    private IServiceScope _serviceScopeForDispatchRWithoutPieline;
    private DispatchR.IMediator _dispatchR;
    private DispatchR.IMediator _dispatchRWithoutPipeline;
    private IMediator _mediatR;
    private IMediator _mediatRWithoutPipeline;
    private static readonly PingDispatchR StaticDispatchR = new();
    private static readonly PingMediatR StaticPingMediatR = new();
    private static readonly PingDispatchRWithOutHandler StaticDispatchRRequestWithOutHandler = new();
    private static readonly PingMediatRWithOutHandler StaticPingMediatRWithOutHandler = new();
    private static List<IServiceScope> ScopesForMediatR { get; set; } = new(TotalSendRequests);
    private static List<IServiceScope> ScopesForMediatRWithoutPipeline { get; set; } = new(TotalSendRequests);
    private static List<IServiceScope> ScopesForDispatchR { get; set; } = new(TotalSendRequests);
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
        withoutPipelineServices.AddDispatchRHandlers(typeof(PingDispatchR).Assembly);
        var buildServicesWithoutPipeline = withoutPipelineServices.BuildServiceProvider();
        _dispatchRWithoutPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<DispatchR.IMediator>();
        _mediatRWithoutPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<MediatR.IMediator>();
        _serviceScopeForMediatRWithoutPipeline = buildServicesWithoutPipeline.CreateScope();
        _serviceScopeForDispatchRWithoutPieline = buildServicesWithoutPipeline.CreateScope();
        ScopesForMediatRWithoutPipeline.Clear();
        ScopesForDispatchRWithoutPipeline.Clear();
        Parallel.For(0, TotalSendRequests, i =>
        {
            ScopesForMediatRWithoutPipeline.Add(buildServicesWithoutPipeline.CreateScope());
            ScopesForDispatchRWithoutPipeline.Add(buildServicesWithoutPipeline.CreateScope());
        });
        
        // with pipeline
        var services = new ServiceCollection();
        services.AddMediatR(cfg =>
        {
            cfg.Lifetime = ServiceLifetime.Scoped;
            cfg.RegisterServicesFromAssemblies(typeof(PingHandlerMediatR).Assembly);
        });
        services.AddTransient<MediatR.IPipelineBehavior<PingMediatR, int>, LoggingBehaviorMediat>();
        services.AddDispatchR(typeof(PingDispatchR).Assembly);
        var buildServices = services.BuildServiceProvider();
        
        _serviceScopeForMediatR = buildServices.CreateScope();
        _serviceScopeForDispatchR = buildServices.CreateScope();
        _dispatchR = buildServices.CreateScope().ServiceProvider.GetRequiredService<DispatchR.IMediator>();
        _mediatR = buildServices.CreateScope().ServiceProvider.GetRequiredService<MediatR.IMediator>();
        ScopesForMediatR.Clear();
        ScopesForDispatchR.Clear();
        Parallel.For(0, TotalSendRequests, i =>
        {
            ScopesForMediatR.Add(buildServices.CreateScope());
            ScopesForDispatchR.Add(buildServices.CreateScope());
        });
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceScopeForMediatR.Dispose();
        _serviceScopeForDispatchR.Dispose();
        _dispatchR = null!;
        _mediatR = null!;
        Parallel.ForEach(ScopesForMediatR, scope => scope.Dispose());
        Parallel.ForEach(ScopesForDispatchR, scope => scope.Dispose());
        
        _serviceScopeForMediatRWithoutPipeline.Dispose();
        _serviceScopeForDispatchRWithoutPieline.Dispose();
        _dispatchRWithoutPipeline = null!;
        _mediatRWithoutPipeline = null!;
        Parallel.ForEach(ScopesForMediatRWithoutPipeline, scope => scope.Dispose());
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
    public Task<int> DispatchR_SendRequest_With_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _dispatchRWithoutPipeline.Send(StaticDispatchRRequestWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return Task.FromResult(0);
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
    public Task<int> DispatchR_SendRequest_With_ExistCommand_ExistMediator()
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
    public async Task<int> DispatchR_SendRequest_With_ExistCommand_GetMediator()
    {
        var result = await _serviceScopeForDispatchRWithoutPieline
            .ServiceProvider
            .GetRequiredService<DispatchR.IMediator>()
            .Send(StaticDispatchR, CancellationToken.None);
        
        return result;
    }

    #endregion
    
    #region SendRequest_With_ExistCommand_ExistMediator_Parallel

    [Benchmark]
    public async Task<int> MediatR___SendRequest_With_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _mediatRWithoutPipeline.Send(StaticPingMediatR, CancellationToken.None);
        });
        
        return result;
    }

    [Benchmark]
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

    [Benchmark]
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

    [Benchmark]
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
    
    // =====================================
    
    #region SendRequest_With_Pipeline_ExistCommand_ExistMediator_WithOutHandler

    [Benchmark]
    public Task<int> MediatR___SendRequest_With_Pipeline_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _mediatR.Send(StaticPingMediatRWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return Task.FromResult(0);
        }
    }

    [Benchmark]
    public Task<int> DispatchR_SendRequest_With_Pipeline_ExistCommand_ExistMediator_WithOut_Handler()
    {
        try
        {
            return _dispatchR.Send(StaticDispatchRRequestWithOutHandler, CancellationToken.None);
        }
        catch
        {
            return Task.FromResult(0);
        }
    }

    #endregion
    
    #region SendRequest_With_Pipeline_ExistCommand_ExistMediator

    [Benchmark]
    public Task<int> MediatR___SendRequest_With_Pipeline_ExistCommand_ExistMediator()
    {
        return _mediatR.Send(StaticPingMediatR, CancellationToken.None);
    }

    [Benchmark]
    public Task<int> DispatchR_SendRequest_With_Pipeline_ExistCommand_ExistMediator()
    {
        return _dispatchR.Send(StaticDispatchR, CancellationToken.None);
    }

    #endregion
    
    #region SendRequest_With_Pipeline_ExistCommand_GetMediator

    [Benchmark]
    public async Task<int> MediatR___SendRequest_With_Pipeline_ExistCommand_GetMediator()
    {
        var result = await _serviceScopeForMediatR
            .ServiceProvider
            .GetRequiredService<MediatR.IMediator>()
            .Send(StaticPingMediatR, CancellationToken.None);
        
        return result;
    }

    [Benchmark]
    public async Task<int> DispatchR_SendRequest_With_Pipeline_ExistCommand_GetMediator()
    {
        var result = await _serviceScopeForDispatchR
            .ServiceProvider
            .GetRequiredService<DispatchR.IMediator>()
            .Send(StaticDispatchR, CancellationToken.None);
        
        return result;
    }

    #endregion
    
    #region SendRequest_With_Pipeline_ExistCommand_ExistMediator_Parallel

    [Benchmark]
    public async Task<int> MediatR___SendRequest_With_Pipeline_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _mediatR.Send(StaticPingMediatR, CancellationToken.None);
        });
        
        return result;
    }

    [Benchmark]
    public async Task<int> DispatchR_SendRequest_With_Pipeline_ExistCommand_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalSendRequests, async (index, ct) =>
        {
            result = await _dispatchR.Send(StaticDispatchR, CancellationToken.None);
        });
        
        return result;
    }

    #endregion
    
    #region SendRequest_With_Pipeline_ExistCommand_GetMediator_ExistScopes_Parallel

    [Benchmark]
    public async Task<int> MediatR___SendRequest_With_Pipeline_ExistCommand_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatR, async (scope, ct) =>
        {
            result = await scope.ServiceProvider.GetRequiredService<MediatR.IMediator>()
                .Send(StaticPingMediatR, CancellationToken.None);
        });
        
        return result;
    }

    [Benchmark]
    public async Task<int> DispatchR_SendRequest_With_Pipeline_ExistCommand_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForDispatchR, async (scope, ct) =>
        {
            result = await scope.ServiceProvider.GetRequiredService<DispatchR.IMediator>()
                .Send(StaticDispatchR, CancellationToken.None);
        });
        
        return result;
    }

    #endregion
}