using DispatchR.Abstractions.Notification;

namespace Sample.DispatchR.Notification;

public sealed record GenericAwareNotification(Guid Id, string Message) : INotification;
