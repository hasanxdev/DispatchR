using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using ZLinq;
using ZLinq.Linq;

namespace DispatchR;

public interface IMediator
{
    Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> command,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new();
}

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<Type, Type[]> HandlerTypesCache = new();

    public Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> command, CancellationToken cancellationToken) where TRequest : class, IRequest, new()
    {
        return SendSimple(command, cancellationToken);
    }
    
    private Task<TResponse> SendSimple<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new()
    {
        IRequestHandler<TRequest, TResponse>? current = null;
        foreach (var handler in serviceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().AsValueEnumerable())
        {
            handler.SetNext(current!);
            current = handler;
        }

        return current!.Handle(Unsafe.As<TRequest>(request), cancellationToken);
    }
    
    public Task<TResponse> SendWithCache<TRequest, TResponse>(IRequest<TRequest, TResponse> command,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new()
    {
        var key = typeof(TRequest);
        ValueEnumerable<FromEnumerable<IRequestHandler<TRequest, TResponse>>, IRequestHandler<TRequest, TResponse>> handlers;
        if (HandlerTypesCache.TryGetValue(key, out var handlerTypes) is false)
        {
            handlers = serviceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().AsValueEnumerable();
            HandlerTypesCache.TryAdd(key, handlers.Select(static h => h.GetType()).ToArray());
        }
        else
        {
            handlers = handlerTypes
                .Select(type => Unsafe.As<IRequestHandler<TRequest, TResponse>>(ActivatorUtilities.CreateInstance(serviceProvider, type)))
                .AsValueEnumerable();
        }
            
        IRequestHandler<TRequest, TResponse>? current = null;
        foreach (var handler in handlers)
        {
            handler.SetNext(current!);
            current = handler;
        }

        return current!.Handle(Unsafe.As<TRequest>(command), cancellationToken);
    }
}