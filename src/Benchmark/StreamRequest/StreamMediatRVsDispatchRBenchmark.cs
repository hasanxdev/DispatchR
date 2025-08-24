using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DispatchR;
using DispatchR.Extensions;
using IMediator = MediatR.IMediator;

namespace Benchmark.StreamRequest;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class StreamMediatRVsDispatchBenchmark
{
    private const int TotalStreamRequests = 5_000;
    private IServiceScope _serviceScopeForMediatRWithoutPipeline;
    private IServiceScope _serviceScopeForMediatSgWithoutPipeline;
    private IServiceScope _serviceScopeForDispatchRWithoutPipeline;
    private DispatchR.IMediator _dispatchRWithoutPipeline;
    private IMediator _mediatRWithoutPipeline;
    private Mediator.IMediator _mediatSgWithoutPipeline;
    private static readonly PingStreamDispatchR StaticStreamDispatchR = new();
    private static readonly PingStreamMediatR StaticPingStreamMediatR = new();
    private static readonly PingStreamMediatSg StaticPingStreamMediatSg = new();
    private static readonly PingStreamDispatchRWithOutHandler StaticStreamDispatchRRequestWithOutHandler = new();
    private static readonly PingStreamMediatRWithOutHandler StaticPingStreamMediatRWithOutHandler = new();
    private static readonly PingStreamMediatSgWithOutHandler StaticPingStreamMediatSgWithOutHandler = new();
    private static List<IServiceScope> ScopesForMediatRWithoutPipeline { get; set; } = new(TotalStreamRequests);
    private static List<IServiceScope> ScopesForMediatSgWithoutPipeline { get; set; } = new(TotalStreamRequests);
    private static List<IServiceScope> ScopesForDispatchRWithoutPipeline { get; set; } = new(TotalStreamRequests);

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
        withoutPipelineServices.AddDispatchR(typeof(PingStreamDispatchR).Assembly, withPipelines: false);
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
        Parallel.For(0, TotalStreamRequests, i =>
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
    
    #region StreamRequest_With_ExistRequest_ExistMediator_WithOutHandler

    [Benchmark(Baseline = true)]
    public async Task<int> MediatR___StreamRequest_With_ExistRequest_ExistMediator_WithOut_Handler()
    {
        try
        {
            var last = 0;
            await foreach (var response in _mediatRWithoutPipeline.CreateStream(StaticPingStreamMediatRWithOutHandler, CancellationToken.None).ConfigureAwait(false))
            {
                last = response;
            }
            
            return last;
        }
        catch
        {
            return 0;
        }
    }
    
    [Benchmark]
    public async Task<int> MediatSG__StreamRequest_With_ExistRequest_ExistMediator_WithOut_Handler()
    {
        try
        {
            var last = 0;
            await foreach (var response in _mediatSgWithoutPipeline.CreateStream(StaticPingStreamMediatSgWithOutHandler, CancellationToken.None).ConfigureAwait(false))
            {
                last = response;
            }

            return last;
        }
        catch
        {
            return 0;
        }
    }

    [Benchmark]
    public async Task<int> DispatchR_StreamRequest_With_ExistRequest_ExistMediator_WithOut_Handler()
    {
        try
        {
            var last = 0;
            await foreach (var response in _dispatchRWithoutPipeline.CreateStream(StaticStreamDispatchRRequestWithOutHandler, CancellationToken.None).ConfigureAwait(false))
            {
                last = response;
            }

            return last;
        }
        catch
        {
            return 0;
        }
    }

    #endregion

    #region StreamRequest_With_ExistRequest_ExistMediator

    [Benchmark]
    public async Task<int> MediatR___StreamRequest_With_ExistRequest_ExistMediator()
    {
        var last = 0;
        await foreach (var response in _mediatRWithoutPipeline.CreateStream(StaticPingStreamMediatR, CancellationToken.None).ConfigureAwait(false))
        {
            last = response;
        }
        return last;
    }
    
    [Benchmark]
    public async Task<int> MediatSG__StreamRequest_With_ExistRequest_ExistMediator()
    {
        var last = 0;
        await foreach (var response in _mediatSgWithoutPipeline.CreateStream(StaticPingStreamMediatSg, CancellationToken.None).ConfigureAwait(false))
        {
            last = response;
        }
        return last;
    }

    [Benchmark]
    public async Task<int> DispatchR_StreamRequest_With_ExistRequest_ExistMediator()
    {
        var last = 0;
        await foreach (var response in _dispatchRWithoutPipeline.CreateStream(StaticStreamDispatchR, CancellationToken.None).ConfigureAwait(false))
        {
            last = response;
        }
        return last;
    }

    #endregion
    
    #region StreamRequest_With_ExistRequest_GetMediator

    [Benchmark]
    public async Task<int> MediatR___StreamRequest_With_ExistRequest_GetMediator()
    {
        var mediator = _serviceScopeForMediatRWithoutPipeline
            .ServiceProvider
            .GetRequiredService<MediatR.IMediator>();;
        
        var last = 0;
        await foreach (var response in mediator.CreateStream(StaticPingStreamMediatR, CancellationToken.None).ConfigureAwait(false))
        {
            last = response;
        }
        return last;
    }
    
    [Benchmark]
    public async Task<int> MediatSG__StreamRequest_With_ExistRequest_GetMediator()
    {
        var mediator = _serviceScopeForMediatSgWithoutPipeline
            .ServiceProvider
            .GetRequiredService<Mediator.IMediator>();
        
        var last = 0;
        await foreach (var response in mediator.CreateStream(StaticPingStreamMediatSg, CancellationToken.None).ConfigureAwait(false))
        {
            last = response;
        }
        return last;
    }

    [Benchmark]
    public async Task<int> DispatchR_StreamRequest_With_ExistRequest_GetMediator()
    {
        var mediator = _serviceScopeForDispatchRWithoutPipeline
            .ServiceProvider
            .GetRequiredService<DispatchR.IMediator>();
        
        var last = 0;
        await foreach (var response in mediator.CreateStream(StaticStreamDispatchR, CancellationToken.None).ConfigureAwait(false))
        {
            last = response;
        }
        return last;
    }

    #endregion
    
    #region StreamRequest_With_ExistRequest_ExistMediator_Parallel

    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> MediatR___StreamRequest_With_ExistRequest_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalStreamRequests, async (index, ct) =>
        {
            await foreach (var response in _mediatRWithoutPipeline.CreateStream(StaticPingStreamMediatR, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }
    
    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> MediatSG__StreamRequest_With_ExistRequest_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalStreamRequests, async (index, ct) =>
        {
            await foreach (var response in _mediatSgWithoutPipeline.CreateStream(StaticPingStreamMediatSg, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }

    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> DispatchR_StreamRequest_With_ExistRequest_ExistMediator_Parallel()
    {
        var result = 0;
        await Parallel.ForAsync(0, TotalStreamRequests, async (index, ct) =>
        {
            await foreach (var response in _dispatchRWithoutPipeline.CreateStream(StaticStreamDispatchR, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }

    #endregion
    
    #region StreamRequest_With_ExistRequest_GetMediator_ExistScopes_Parallel

    [Benchmark(OperationsPerInvoke = TotalStreamRequests)]
    public async Task<int> MediatR___StreamRequest_With_ExistRequest_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatRWithoutPipeline, async (scope, ct) =>
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
    public async Task<int> MediatSG__StreamRequest_With_ExistRequest_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForMediatSgWithoutPipeline, async (scope, ct) =>
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
    public async Task<int> DispatchR_StreamRequest_With_ExistRequest_GetMediator_ExistScopes_Parallel()
    {
        var result = 0;
        await Parallel.ForEachAsync(ScopesForDispatchRWithoutPipeline, async (scope, ct) =>
        {
            var mediator = scope.ServiceProvider.GetRequiredService<DispatchR.IMediator>();
            await foreach (var response in mediator.CreateStream(StaticStreamDispatchR, CancellationToken.None).ConfigureAwait(false))
            {
                result = response;
            }
        });
        
        return result;
    }

    #endregion
}