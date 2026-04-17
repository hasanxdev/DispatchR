using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class OpenGenericTargetNotificationHandler : INotificationHandler<OpenGenericTargetNotification>
{
    public ValueTask Handle(OpenGenericTargetNotification request, CancellationToken cancellationToken)
    {
        OpenGenericNotificationExecutionStore.Increment($"specific:{nameof(OpenGenericTargetNotification)}");
        return ValueTask.CompletedTask;
    }
}
