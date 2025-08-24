using DispatchR.Abstractions.Notification;
using Sample.MediatR.Notification;

namespace Sample.DispatchR.Notification;

public sealed class NotificationTwoHandler(ILogger<NotificationTwoHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification two");
        return ValueTask.CompletedTask;
    }
}