using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace DispatchR;

public interface IMediator
{
    Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> command,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new();
}

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<(Type, Type), List<Type>> HandlerTypesCache = new();

    public Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> command, CancellationToken cancellationToken) where TRequest : class, IRequest, new()
    {
        return SendSimple(command, cancellationToken);
    }
    
    public Task<TResponse> SendSimple<TRequest, TResponse>(IRequest<TRequest, TResponse> command,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new()
    {
        IRequestHandler<TRequest, TResponse>? current = null;
        var handlers = serviceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().ToArray();
        for (var index = 0; index < handlers.Length; index++)
        {
            var handler = handlers[index];
            if (index > 0)
            {
                handler.SetNext(current!);
            }

            current = handler;
        }

        return current!.Handle((TRequest)command, cancellationToken);
    }
    
    public Task<TResponse> SendWithCache<TRequest, TResponse>(IRequest<TRequest, TResponse> command,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new()
    {
        var key = (typeof(TRequest), typeof(TResponse));
        IRequestHandler<TRequest, TResponse>[] handlers;
        if (HandlerTypesCache.TryGetValue(key, out var handlerTypes) is false)
        {
            handlers = serviceProvider.GetServices<IRequestHandler<TRequest, TResponse>>().ToArray();
            HandlerTypesCache.TryAdd(key, handlers.Select(h => h.GetType()).ToList());
        }
        else
        {
            handlers = handlerTypes.Select(type => 
                    (IRequestHandler<TRequest, TResponse>)ActivatorUtilities.CreateInstance(serviceProvider, type))
                .ToArray();
        }
            
        IRequestHandler<TRequest, TResponse>? current = null;
        for (var index = 0; index < handlers.Length; index++)
        {
            var handler = handlers[index];
            if (index > 0)
            {
                handler.SetNext(current!);
            }

            current = handler;
        }

        return current!.Handle((TRequest)command, cancellationToken);
    }
}