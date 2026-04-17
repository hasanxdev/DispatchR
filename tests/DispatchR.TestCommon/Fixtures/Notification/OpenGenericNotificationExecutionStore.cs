using System.Collections.Concurrent;

namespace DispatchR.TestCommon.Fixtures.Notification;

public static class OpenGenericNotificationExecutionStore
{
    private static readonly ConcurrentDictionary<string, int> Counters = new();

    public static void Reset()
    {
        Counters.Clear();
    }

    public static void Increment(string key)
    {
        Counters.AddOrUpdate(key, 1, (_, current) => current + 1);
    }

    public static int Count(string key)
    {
        return Counters.TryGetValue(key, out var count) ? count : 0;
    }
}
