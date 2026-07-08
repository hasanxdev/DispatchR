using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public abstract class AbstractNotificationHandler : INotificationHandler<OpenGenericTargetNotification>
{
    public abstract ValueTask Handle(OpenGenericTargetNotification request, CancellationToken cancellationToken);
}