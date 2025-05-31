﻿using Benchmark.Notification.MultiHandlers;
using Benchmark.Notification.MultiHandlersAsync;
using Benchmark.Notification.SingleHandler;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DispatchR;

namespace Benchmark.Notification;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class NotificationBenchmarks
{
    private IServiceProvider _serviceProvider;
    private IServiceScope _serviceScope;
    private Mediator.IMediator _mediator;
    private DispatchR.Requests.IMediator _dispatchR;
    private Mediator.Mediator _concreteMediator;
    private MediatR.IMediator _mediatr;
    private SingleHandler.SingleHandler _singleHandler;
    private SingleHandlerNotification _singleHandlerNotification;
    private MultiHandler0 _multiHandler0;
    private MultiHandler1 _multiHandler1;
    private MultiHandler2 _multiHandler2;
    private MultiHandlersNotification _multiHandlersNotification;
    private MultiHandlerAsync0 _multiHandlerAsync0;
    private MultiHandlerAsync1 _multiHandlerAsync1;
    private MultiHandlerAsync2 _multiHandlerAsync2;
    private MultiHandlersAsyncNotification _multiHandlersAsyncNotification;

    public enum ScenarioType
    {
        SingleHandlerSync,
        MultiHandlersSync,
        MultiHandlersAsync,
    }

    [ParamsAllValues]
    public ScenarioType Scenario { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediator(opts =>
        {
            opts.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddDispatchR(typeof(SingleHandler.SingleHandler).Assembly);
        services.AddMediatR(opts =>
        {
            opts.Lifetime = ServiceLifetime.Scoped;
            opts.RegisterServicesFromAssembly(typeof(SingleHandler.SingleHandler).Assembly);
        });

        _serviceProvider = services.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateScope();
        _serviceProvider = _serviceScope.ServiceProvider;

        _mediator = _serviceProvider.GetRequiredService<Mediator.IMediator>();
        _dispatchR = _serviceProvider.GetRequiredService<DispatchR.Requests.IMediator>();
        _concreteMediator = _serviceProvider.GetRequiredService<Mediator.Mediator>();
        _mediatr = _serviceProvider.GetRequiredService<MediatR.IMediator>();

        _singleHandler = _serviceProvider.GetRequiredService<SingleHandler.SingleHandler>();
        _singleHandlerNotification = new(Guid.NewGuid());

        _multiHandler0 = _serviceProvider.GetRequiredService<MultiHandler0>();
        _multiHandler1 = _serviceProvider.GetRequiredService<MultiHandler1>();
        _multiHandler2 = _serviceProvider.GetRequiredService<MultiHandler2>();
        _multiHandlersNotification = new(Guid.NewGuid());

        _multiHandlerAsync0 = _serviceProvider.GetRequiredService<MultiHandlerAsync0>();
        _multiHandlerAsync1 = _serviceProvider.GetRequiredService<MultiHandlerAsync1>();
        _multiHandlerAsync2 = _serviceProvider.GetRequiredService<MultiHandlerAsync2>();
        _multiHandlersAsyncNotification = new(Guid.NewGuid());
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (_serviceScope is not null)
            _serviceScope.Dispose();
        else
            (_serviceProvider as IDisposable)?.Dispose();
    }

    [Benchmark]
    public Task Publish_Notification_MediatR()
    {
        return Scenario switch
        {
            ScenarioType.SingleHandlerSync => _mediatr.Publish(_singleHandlerNotification),
            ScenarioType.MultiHandlersSync => _mediatr.Publish(_multiHandlersNotification),
            ScenarioType.MultiHandlersAsync => _mediatr.Publish(_multiHandlersAsyncNotification),
        };
    }
    
    [Benchmark]
    public ValueTask Publish_Notification_DispatchR()
    {
        return Scenario switch
        {
            ScenarioType.SingleHandlerSync => _dispatchR.Publish(_singleHandlerNotification, CancellationToken.None),
            ScenarioType.MultiHandlersSync => _dispatchR.Publish(_multiHandlersNotification, CancellationToken.None),
            ScenarioType.MultiHandlersAsync => _dispatchR.Publish(_multiHandlersAsyncNotification, CancellationToken.None),
        };
    }

    [Benchmark]
    public ValueTask Publish_Notification_IMediator()
    {
        return Scenario switch
        {
            ScenarioType.SingleHandlerSync => _mediator.Publish(_singleHandlerNotification),
            ScenarioType.MultiHandlersSync => _mediator.Publish(_multiHandlersNotification),
            ScenarioType.MultiHandlersAsync => _mediator.Publish(_multiHandlersAsyncNotification),
        };
    }

    [Benchmark]
    public ValueTask Publish_Notification_Mediator()
    {
        return Scenario switch
        {
            ScenarioType.SingleHandlerSync => _concreteMediator.Publish(_singleHandlerNotification),
            ScenarioType.MultiHandlersSync => _concreteMediator.Publish(_multiHandlersNotification),
            ScenarioType.MultiHandlersAsync => _concreteMediator.Publish(_multiHandlersAsyncNotification),
        };
    }

    [Benchmark(Baseline = true)]
    public ValueTask Publish_Notification_Baseline()
    {
        switch (Scenario)
        {
            case ScenarioType.SingleHandlerSync:
                return _singleHandler.Handle(_singleHandlerNotification, default);
            case ScenarioType.MultiHandlersSync:
                _multiHandler0.Handle(_multiHandlersNotification, default).GetAwaiter().GetResult();
                _multiHandler1.Handle(_multiHandlersNotification, default).GetAwaiter().GetResult();
                _multiHandler2.Handle(_multiHandlersNotification, default).GetAwaiter().GetResult();
                return default;
            case ScenarioType.MultiHandlersAsync:
                return AwaitMultipleHandlersAsync();
        }
    
        return default;
    
        async ValueTask AwaitMultipleHandlersAsync()
        {
            await _multiHandlerAsync0.Handle(_multiHandlersAsyncNotification, default);
            await _multiHandlerAsync1.Handle(_multiHandlersAsyncNotification, default);
            await _multiHandlerAsync2.Handle(_multiHandlersAsyncNotification, default);
        }
    }
}
