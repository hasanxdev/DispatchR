using DispatchR.Abstractions.Notification;

namespace Sample.DispatchR.Notification;

/// <summary>
/// Open-generic notification handler — handles every INotification published through DispatchR.
/// Useful for cross-cutting concerns such as logging, auditing or telemetry.
/// </summary>
public sealed class AllNotificationsLogger<TNotification>(ILogger<AllNotificationsLogger<TNotification>> logger)
    : INotificationHandler<TNotification>
    where TNotification : INotification
{
    public ValueTask Handle(TNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Generic] Received notification of type {NotificationType}: {@Notification}",
            typeof(TNotification).Name, notification);
        return ValueTask.CompletedTask;
    }
}
