using Mediator;

namespace Benchmark.Notification.SingleHandler;

public sealed class SingleHandler
    : INotificationHandler<SingleHandlerNotification>,
        MediatR.INotificationHandler<SingleHandlerNotification>,
        DispatchR.Abstractions.Notification.INotificationHandler<SingleHandlerNotification>
{
    public ValueTask Handle(SingleHandlerNotification notification, CancellationToken cancellationToken) => default;

    Task MediatR.INotificationHandler<SingleHandlerNotification>.Handle(
        SingleHandlerNotification notification,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;
}