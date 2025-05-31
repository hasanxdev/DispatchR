using System.Runtime.CompilerServices;
using DispatchR.Requests.Send;

namespace DispatchR.Requests.Notification;

public interface INotificationHandler<in TRequestEvent> : IRequestHandler where TRequestEvent : INotification
{
    ValueTask Handle(TRequestEvent request, CancellationToken cancellationToken);
}