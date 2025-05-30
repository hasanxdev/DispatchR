using DispatchR.Requests.Notification;

namespace Sample.DispatchR.Notification;

public sealed class NotificationTwoHandler(ILogger<NotificationTwoHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    public INotificationHandler<MultiHandlersNotification>? NextNotification { get; set; }

    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification two");
        return ValueTask.CompletedTask;
    }
}