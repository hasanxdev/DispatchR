using DispatchR.Requests.Notification;

namespace Sample.DispatchR.Notification;

public sealed class NotificationThreeHandler(ILogger<NotificationThreeHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    public INotificationHandler<MultiHandlersNotification>? NextNotification { get; set; }

    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification three");
        return ValueTask.CompletedTask;
    }
}