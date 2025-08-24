using DispatchR.Abstractions.Notification;
using Sample.MediatR.Notification;

namespace Sample.DispatchR.Notification;

public sealed class NotificationThreeHandler(ILogger<NotificationThreeHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification three");
        return ValueTask.CompletedTask;
    }
}