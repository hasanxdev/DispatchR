using MediatR;

namespace Sample.MediatR.Notification;

public sealed class NotificationOneHandler(ILogger<NotificationOneHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    Task INotificationHandler<MultiHandlersNotification>.Handle(MultiHandlersNotification notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification one");
        return Task.CompletedTask;
    }
}