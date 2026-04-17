using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed record OpenGenericOnlyNotification(Guid Id) : INotification;
