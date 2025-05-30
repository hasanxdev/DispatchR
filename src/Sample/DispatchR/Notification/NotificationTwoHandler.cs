using DispatchR.Requests.Notification;

namespace Sample.DispatchR.Notification;

public sealed class NotificationTwoHandler(ILogger<NotificationTwoHandler> logger) : INotificationHandler<MultiHandlersNotification, Task>
{
    public Task Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification two");
        return Task.CompletedTask;
    }
}