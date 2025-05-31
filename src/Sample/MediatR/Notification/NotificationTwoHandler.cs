using MediatR;

namespace Sample.MediatR.Notification;

public sealed class NotificationTwoHandler(ILogger<NotificationTwoHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    Task INotificationHandler<MultiHandlersNotification>.Handle(MultiHandlersNotification notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification two");
        return Task.CompletedTask;
    }
}