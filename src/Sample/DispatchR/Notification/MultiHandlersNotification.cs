using DispatchR.Requests.Notification;

namespace Sample.DispatchR.Notification;

public sealed record MultiHandlersNotification(Guid Id) : INotification;