using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class MultiNotificationHandler :
    INotificationHandler<MultiHandlersNotification>,
    INotificationHandler<MultiHandlersNotification2>
{
    public ValueTask Handle(MultiHandlersNotification request, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask Handle(MultiHandlersNotification2 request, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
