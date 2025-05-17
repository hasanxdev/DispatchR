using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DispatchR;
using Mediator;
using IMediator = MediatR.IMediator;

namespace Benchmark.StreamRequest;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class StreamMediatRVsDispatchWithPipelineRBenchmark
{
    private const int TotalStreamRequests = 5_000;
    private IServiceScope _serviceScopeForMediatRWithPipeline;
    private IServiceScope _serviceScopeForMediatSgWithPipeline;
    private IServiceScope _serviceScopeForDispatchRWithPipeline;
    private DispatchR.Requests.IMediator _dispatchRWithPipeline;
    private IMediator _mediatRWithPipeline;
    private Mediator.IMediator _mediatSgWithPipeline;
    private static readonly PingStreamDispatchR StaticStreamDispatchR = new();
    private static readonly PingStreamMediatR StaticPingStreamMediatR = new();
    private static readonly PingStreamMediatSg StaticPingStreamMediatSg = new();
    private static readonly PingStreamDispatchRWithOutHandler StaticStreamDispatchRRequestWithOutHandler = new();
    private static readonly PingStreamMediatRWithOutHandler StaticPingStreamMediatRWithOutHandler = new();
    private static readonly PingStreamMediatSgWithOutHandler StaticPingStreamMediatSgWithOutHandler = new();
    private static List<IServiceScope> ScopesForMediatRWithPipeline { get; set; } = new(TotalStreamRequests);
    private static List<IServiceScope> ScopesForMediatSgWithPipeline { get; set; } = new(TotalStreamRequests);
    private static List<IServiceScope> ScopesForDispatchRWithPipeline { get; set; } = new(TotalStreamRequests);

    [GlobalSetup]
    public void Setup()
    {
        var withPipelineServices = new ServiceCollection();
        
        withPipelineServices.AddMediatR(cfg =>
        {
            cfg.Lifetime = ServiceLifetime.Scoped;
            cfg.RegisterServicesFromAssemblies(typeof(PingHandlerMediatR).Assembly);
        });
        withPipelineServices.AddScoped<MediatR.IStreamPipelineBehavior<PingStreamMediatR, int>, LoggingBehaviorMediat>();

        withPipelineServices.AddMediator((MediatorOptions options) =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            // options.PipelineBehaviors = [typeof(LoggingBehaviorMediatSG)];
        });
        withPipelineServices.AddScoped<Mediator.IStreamPipelineBehavior<PingStreamMediatSg, int>, LoggingBehaviorMediatSg>();

        withPipelineServices.AddDispatchR(typeof(PingStreamDispatchR).Assembly);
        var buildServicesWithoutPipeline = withPipelineServices.BuildServiceProvider();
        _dispatchRWithPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<DispatchR.Requests.IMediator>();
        _mediatRWithPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<MediatR.IMediator>();
        _mediatSgWithPipeline = buildServicesWithoutPipeline.CreateScope().ServiceProvider.GetRequiredService<Mediator.IMediator>();
        _serviceScopeForMediatRWithPipeline = buildServicesWithoutPipeline.CreateScope();
        _serviceScopeForMediatSgWithPipeline = buildServicesWithoutPipeline.CreateScope();
        _serviceScopeForDispatchRWithPipeline = buildServicesWithoutPipeline.CreateScope();
        ScopesForMediatRWithPipeline.Clear();
        ScopesForMediatSgWithPipeline.Clear();
        ScopesForDispatchRWithPipeline.Clear();
        Parallel.For(0, TotalStreamRequests, i =>
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
    
    #region StreamRequest_With_Pipeline_ExistRequest_ExistMediator_WithOutHandler
    
    [Benchmark(Baseline = true)]
    public async Task<int> MediatR___StreamRequest_ExistRequest_ExistMediator_WithOut_Handler()
    {
        try
        {
            var result = 0;
            await foreach (var response in _mediatRWithPipeline.CreateStream(StaticPingStreamMediatRWithOutHandler, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
            
            return result;
        }
        catch
        {
            return 0;
        }
    }
    
    [Benchmark]
    public async Task<int> MediatSG__StreamRequest_ExistRequest_ExistMediator_WithOut_Handler()
    {
        try
        {
            var result = 0;
            await foreach (var response in _mediatSgWithPipeline.CreateStream(StaticPingStreamMediatSgWithOutHandler, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
            
            return result;
        }
        catch
        {
            return 0;
        }
    }
    
    [Benchmark]
    public async Task<int> DispatchR_StreamRequest_ExistRequest_ExistMediator_WithOut_Handler()
    {
        try
        {
            var result = 0;
            await foreach (var response in _dispatchRWithPipeline.CreateStream(StaticStreamDispatchRRequestWithOutHandler, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
            
            return result;
        }
        catch
        {
            return 0;
        }
    }
    
    #endregion
    
    #region StreamRequest_ExistRequest_ExistMediator
    
    [Benchmark]
    public async Task<int> MediatR___StreamRequest_ExistRequest_ExistMediator()
    {
        var result = 0;
        await foreach (var response in _mediatRWithPipeline.CreateStream(StaticPingStreamMediatR, CancellationToken.None).ConfigureAwait(false))
        {
            result = response;
        }
            
        return result;
    }
    
    [Benchmark]
    public async Task<int> MediatSG__StreamRequest_ExistRequest_ExistMediator()
    {
        var result = 0;
        await foreach (var response in _mediatSgWithPipeline.CreateStream(StaticPingStreamMediatSg, CancellationToken.None).ConfigureAwait(false))
        {
            result = response;
        }
            
        return result;
    }
    
    [Benchmark]
    public async Task<int> DispatchR_StreamRequest_ExistRequest_ExistMediator()
    {
        var result = 0;
        await foreach (var response in _dispatchRWithPipeline.CreateStream(StaticStreamDispatchR, CancellationToken.None).ConfigureAwait(false))
        {
            result = response;
        }
            
        return result;
    }
    
    #endregion
    
    #region StreamRequest_ExistRequest_GetMediator
    
    [Benchmark]
    public async Task<int> MediatR___StreamRequest_ExistRequest_GetMediator()
    {
        var mediator = _serviceScopeForMediatRWithPipeline
            .ServiceProvider
            .GetRequiredService<MediatR.IMediator>();
        
        var result = 0;
        await foreach (var response in mediator.CreateStream(StaticPingStreamMediatR, CancellationToken.None).ConfigureAwait(false))
        {
            result = response;
        }
            
        return result;
    }
    
    [Benchmark]
    public async Task<int> MediatSG__StreamRequest_ExistRequest_GetMediator()
    {
        var mediator = _serviceScopeForMediatSgWithPipeline
            .ServiceProvider
            .GetRequiredService<Mediator.IMediator>();
        
        var result = 0;
        await foreach (var response in mediator.CreateStream(StaticPingStreamMediatSg, CancellationToken.None).ConfigureAwait(false))
        {
            result = response;
        }
    
        return result;
    }
    
    [Benchmark]
    public async Task<int> DispatchR_StreamRequest_ExistRequest_GetMediator()
    {
        var mediator = _serviceScopeForDispatchRWithPipeline
            .ServiceProvider
            .GetRequiredService<DispatchR.Requests.IMediator>();
        
        var result = 0;
        await foreach (var response in mediator.CreateStream(StaticStreamDispatchR, CancellationToken.None).ConfigureAwait(false))
        {
            result = response;
        }
    
        return result;
    }
    
    #endregion
    
    #region StreamRequest_ExistRequest_ExistMediator_Parallel
    
    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> MediatR___StreamRequest_ExistRequest_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalStreamRequests, async (index, ct) =>
        {
            await foreach (var response in _mediatRWithPipeline.CreateStream(StaticPingStreamMediatR, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> MediatSG__StreamRequest_ExistRequest_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalStreamRequests, async (index, ct) =>
        {
            await foreach (var response in _mediatSgWithPipeline.CreateStream(StaticPingStreamMediatSg, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> DispatchR_StreamRequest_ExistRequest_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalStreamRequests, async (index, ct) =>
        {
            await foreach (var response in _dispatchRWithPipeline.CreateStream(StaticStreamDispatchR, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }
    
    #endregion
    
    #region StreamRequest_ExistRequest_GetMediator_ExistScopes_Parallel
    
    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> MediatR___StreamRequest_ExistRequest_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatRWithPipeline, async (scope, ct) =>
        {
            var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
            await foreach (var response in mediator.CreateStream(StaticPingStreamMediatR, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> MediatSG__StreamRequest_ExistRequest_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatSgWithPipeline, async (scope, ct) =>
        {
            var mediator = scope.ServiceProvider.GetRequiredService<Mediator.IMediator>();
            
            await foreach (var response in mediator.CreateStream(StaticPingStreamMediatSg, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> DispatchR_StreamRequest_ExistRequest_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForDispatchRWithPipeline, async (scope, ct) =>
        {
            var mediator = scope.ServiceProvider.GetRequiredService<DispatchR.Requests.IMediator>();
            await foreach (var response in mediator.CreateStream(StaticStreamDispatchR, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }
    
    #endregion
}