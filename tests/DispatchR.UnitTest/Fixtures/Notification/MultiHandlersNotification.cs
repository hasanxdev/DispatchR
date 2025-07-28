using DispatchR.Requests.Notification;

namespace DispatchR.UnitTest.Fixtures.Notification;

public sealed record MultiHandlersNotification(Guid Id) : INotification;