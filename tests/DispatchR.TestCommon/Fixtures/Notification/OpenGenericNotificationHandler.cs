using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class OpenGenericNotificationHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    public ValueTask Handle(TNotification request, CancellationToken cancellationToken)
    {
        OpenGenericNotificationExecutionStore.Increment($"generic:{typeof(TNotification).Name}");
        return ValueTask.CompletedTask;
    }
}
