using MediatR;

namespace Sample.MediatR.Notification;

public sealed record MultiHandlersNotification(Guid Id) : INotification;