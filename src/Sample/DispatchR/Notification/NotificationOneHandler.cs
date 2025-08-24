using DispatchR.Abstractions.Notification;

namespace Sample.DispatchR.Notification;

public sealed class NotificationOneHandler(ILogger<NotificationOneHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification one");
        return ValueTask.CompletedTask;
    }
}