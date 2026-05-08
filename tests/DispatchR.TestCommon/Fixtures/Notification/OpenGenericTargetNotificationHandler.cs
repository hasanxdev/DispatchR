using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class OpenGenericTargetNotificationHandler : INotificationHandler<OpenGenericTargetNotification>
{
    private static readonly OpenGenericNotificationExecutionStore FallbackStore = new();
    private readonly OpenGenericNotificationExecutionStore _store;

    public OpenGenericTargetNotificationHandler(OpenGenericNotificationExecutionStore? store = null)
    {
        _store = store ?? FallbackStore;
    }

    public ValueTask Handle(OpenGenericTargetNotification request, CancellationToken cancellationToken)
    {
        _store.Increment($"specific:{nameof(OpenGenericTargetNotification)}");
        return ValueTask.CompletedTask;
    }
}
