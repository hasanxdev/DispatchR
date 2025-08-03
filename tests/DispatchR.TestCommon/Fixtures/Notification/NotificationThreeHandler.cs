using DispatchR.Requests.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class NotificationThreeHandler() : INotificationHandler<MultiHandlersNotification>
{
    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}