using DispatchR.Requests.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed record MultiHandlersNotification(Guid Id) : INotification;