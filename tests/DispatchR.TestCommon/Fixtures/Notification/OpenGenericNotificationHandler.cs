using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class OpenGenericNotificationHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    private static readonly OpenGenericNotificationExecutionStore FallbackStore = new();
    private readonly OpenGenericNotificationExecutionStore _store;

    public OpenGenericNotificationHandler(OpenGenericNotificationExecutionStore? store = null)
    {
        _store = store ?? FallbackStore;
    }

    public ValueTask Handle(TNotification request, CancellationToken cancellationToken)
    {
        _store.Increment($"generic:{typeof(TNotification).Name}");
        return ValueTask.CompletedTask;
    }
}
