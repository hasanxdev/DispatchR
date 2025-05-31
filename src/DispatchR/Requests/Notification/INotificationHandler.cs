using System.Runtime.CompilerServices;
using DispatchR.Requests.Send;

namespace DispatchR.Requests.Notification;

public interface INotificationHandler<in TRequest> : IRequestHandler where TRequest : INotification
{
    ValueTask Handle(TRequest request, CancellationToken cancellationToken);
}