using DispatchR.Abstractions.Notification;

namespace Sample.DispatchR.Notification;

public sealed record MultiHandlersNotification(Guid Id) : INotification;