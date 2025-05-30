using DispatchR.Requests.Notification;

namespace Sample.DispatchR.Notification;

public sealed class NotificationThreeHandler(ILogger<NotificationThreeHandler> logger) : INotificationHandler<MultiHandlersNotification, ValueTask>
{
    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification three");
        return ValueTask.CompletedTask;
    }
}