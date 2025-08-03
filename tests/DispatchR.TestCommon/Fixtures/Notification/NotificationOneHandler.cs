using DispatchR.Requests.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class NotificationOneHandler() : INotificationHandler<MultiHandlersNotification>
{
    public async ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
    }
}