using DispatchR.Requests.Notification;

namespace DispatchR.UnitTest.Fixtures.Notification;

public sealed class NotificationThreeHandler() : INotificationHandler<MultiHandlersNotification>
{
    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}