using MediatR;

namespace Sample.MediatR.Notification;

public sealed class NotificationThreeHandler(ILogger<NotificationThreeHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    Task INotificationHandler<MultiHandlersNotification>.Handle(MultiHandlersNotification notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification three");
        return Task.CompletedTask;
    }
}