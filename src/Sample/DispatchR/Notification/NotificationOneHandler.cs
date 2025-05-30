using DispatchR.Requests.Notification;

namespace Sample.DispatchR.Notification;

public sealed class NotificationOneHandler(ILogger<NotificationOneHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    public void Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification one");
    }
}