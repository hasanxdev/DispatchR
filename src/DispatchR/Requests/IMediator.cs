﻿using System.Runtime.CompilerServices;
using DispatchR.Requests.Notification;
using DispatchR.Requests.Send;
using DispatchR.Requests.Stream;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR.Requests;

public interface IMediator
{
    TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest;

    IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IStreamRequest;

    ValueTask Publish<TNotification>(TNotification request, CancellationToken cancellationToken)
        where TNotification : INotification;
}

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest
    {
        return serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .Handle(Unsafe.As<TRequest>(request), cancellationToken);
    }

    public IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request, 
        CancellationToken cancellationToken) where TRequest : class, IStreamRequest
    {
        return serviceProvider.GetRequiredService<IStreamRequestHandler<TRequest, TResponse>>()
            .Handle(Unsafe.As<TRequest>(request), cancellationToken);
    }

    public async ValueTask Publish<TNotification>(TNotification request, CancellationToken cancellationToken) where TNotification : INotification
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
}