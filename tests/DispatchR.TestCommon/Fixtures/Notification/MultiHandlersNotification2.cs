using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed record MultiHandlersNotification2(Guid Id) : INotification;