using System.Runtime.CompilerServices;
using DispatchR.Requests.Notification;
using DispatchR.Requests.Send;
using DispatchR.Requests.Stream;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR.Requests;

public interface IMediator
{
    TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new();

    IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IStreamRequest, new();
    
    Task Publish<TNotification>(TNotification request, CancellationToken cancellationToken)
        where TNotification : INotification;
}

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new()
    {
        return serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .Handle(Unsafe.As<TRequest>(request), cancellationToken);
    }

    public IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request, 
        CancellationToken cancellationToken) where TRequest : class, IStreamRequest, new()
    {
        return serviceProvider.GetRequiredService<IStreamRequestHandler<TRequest, TResponse>>()
            .Handle(Unsafe.As<TRequest>(request), cancellationToken);
    }

    public async Task Publish<TNotification>(TNotification request, CancellationToken cancellationToken) where TNotification : INotification
    {
        using var taskHandlers = serviceProvider.GetServices<INotificationHandler<TNotification, Task>>().GetEnumerator();
        using var valueTaskHandlers = serviceProvider.GetServices<INotificationHandler<TNotification, ValueTask>>().GetEnumerator();
        using var syncHandlers = serviceProvider.GetServices<INotificationHandler<TNotification>>().GetEnumerator();
        
        while (taskHandlers.MoveNext())
        {
            var handler = taskHandlers.Current;
            await handler.Handle(request, cancellationToken);
        }
        
        while (valueTaskHandlers.MoveNext())
        {
            var handler = valueTaskHandlers.Current;
            await handler.Handle(request, cancellationToken);
        }

        while (syncHandlers.MoveNext())
        {
            var handler = syncHandlers.Current;
            handler.Handle(request, cancellationToken);
        }
    }
}