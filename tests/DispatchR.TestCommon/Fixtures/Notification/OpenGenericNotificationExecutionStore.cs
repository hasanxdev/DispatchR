using System.Collections.Concurrent;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class OpenGenericNotificationExecutionStore
{
    private readonly ConcurrentDictionary<string, int> _counters = new();

    public void Increment(string key)
    {
        _counters.AddOrUpdate(key, 1, (_, current) => current + 1);
    }

    public int Count(string key)
    {
        return _counters.TryGetValue(key, out var count) ? count : 0;
    }
}
