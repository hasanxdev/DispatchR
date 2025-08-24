using DispatchR.Abstractions.Send;

namespace DispatchR.Abstractions.Notification;

public interface INotificationHandler<in TRequestEvent> : IRequestHandler where TRequestEvent : INotification
{
    ValueTask Handle(TRequestEvent request, CancellationToken cancellationToken);
}