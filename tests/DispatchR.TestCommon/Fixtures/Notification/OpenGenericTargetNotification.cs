using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed record OpenGenericTargetNotification(Guid Id) : INotification;
