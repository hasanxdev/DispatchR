using Mediator;

namespace Benchmark.Notification.MultiHandlers;

public sealed class MultiHandler0
    : INotificationHandler<MultiHandlersNotification>,
        MediatR.INotificationHandler<MultiHandlersNotification>,
        DispatchR.Abstractions.Notification.INotificationHandler<MultiHandlersNotification>
{
    public ValueTask Handle(MultiHandlersNotification notification, CancellationToken cancellationToken) => default;

    Task MediatR.INotificationHandler<MultiHandlersNotification>.Handle(
        MultiHandlersNotification notification,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;
}