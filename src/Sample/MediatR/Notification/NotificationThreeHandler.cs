using MediatR;

namespace Sample.MediatR.Notification;

public sealed class NotificationThreeHandler(ILogger<NotificationThreeHandler> logger) : INotificationHandler<MultiHandlersNotification>
{
    public Task Handle(MultiHandlersNotification notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification three");
        return Task.CompletedTask;
    }
}