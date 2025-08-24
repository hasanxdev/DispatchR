using Mediator;

namespace Benchmark.Notification.MultiHandlersAsync;

public sealed class MultiHandlerAsync0
    : INotificationHandler<MultiHandlersAsyncNotification>,
        MediatR.INotificationHandler<MultiHandlersAsyncNotification>,
        DispatchR.Abstractions.Notification.INotificationHandler<MultiHandlersAsyncNotification>
{
    public async ValueTask Handle(MultiHandlersAsyncNotification notification, CancellationToken cancellationToken) =>
        await Task.Yield();

    async Task MediatR.INotificationHandler<MultiHandlersAsyncNotification>.Handle(
        MultiHandlersAsyncNotification notification,
        CancellationToken cancellationToken
    ) => await Task.Yield();
}