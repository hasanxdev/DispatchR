using System.Runtime.CompilerServices;
using DispatchR.Abstractions.Notification;
using DispatchR.Abstractions.Send;
using DispatchR.Abstractions.Stream;
using DispatchR.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR;

public interface IMediator
{
    TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest;

    IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IStreamRequest;

    ValueTask Publish<TNotification>(TNotification request, CancellationToken cancellationToken)
        where TNotification : INotification;
    
    /// <summary>
    /// This method is not recommended for performance-critical scenarios.  
    /// Use it only if it is strictly necessary, as its performance is lower compared  
    /// to similar methods in terms of both memory usage and CPU consumption.  
    /// </summary>
    /// <param name="request">
    /// An object that implements INotification
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete(message: "This method has performance issues. Use only if strictly necessary", 
        error: false, 
        DiagnosticId = Constants.DiagnosticPerformanceIssue)]
    ValueTask Publish(object request, CancellationToken cancellationToken);
}

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest
    {
        try
        {
            return serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
                .Handle(Unsafe.As<TRequest>(request), cancellationToken);
        }
        catch (Exception e) when (e.Message.Contains("No service for type", StringComparison.OrdinalIgnoreCase))
        {
            throw new HandlerNotFoundException<TRequest, TResponse>();
        }
    }

    public IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IStreamRequest
    {
        return serviceProvider.GetRequiredService<IStreamRequestHandler<TRequest, TResponse>>()
            .Handle(Unsafe.As<TRequest>(request), cancellationToken);
    }

    public async ValueTask Publish<TNotification>(TNotification request, CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var notificationsInDi = serviceProvider.GetRequiredService<IEnumerable<INotificationHandler<TNotification>>>();

        var notifications = Unsafe.As<INotificationHandler<TNotification>[]>(notificationsInDi);
        foreach (var notification in notifications)
        {
            var valueTask = notification.Handle(request, cancellationToken);
            if (valueTask.IsCompletedSuccessfully is false)
            {
                await valueTask;
            }
        }
    }

    public async ValueTask Publish(object request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(requestType);

        var notificationsInDi = serviceProvider.GetServices(handlerType);

        foreach (var handler in notificationsInDi)
        {
            var handleMethod = handlerType.GetMethod(nameof(INotificationHandler<INotification>.Handle));
            ArgumentNullException.ThrowIfNull(handleMethod);

            var valueTask = (ValueTask?)handleMethod.Invoke(handler, [request, cancellationToken]);
            ArgumentNullException.ThrowIfNull(valueTask);

            if (!valueTask.Value.IsCompletedSuccessfully)
                await valueTask.Value;
        }
    }
}