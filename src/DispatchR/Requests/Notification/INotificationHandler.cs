using DispatchR.Requests.Send;

namespace DispatchR.Requests.Notification;

public interface INotificationHandler<TRequest> : IRequestHandler where TRequest : INotification
{
    void Handle(TRequest request, CancellationToken cancellationToken);
}

public interface INotificationHandler<TRequest, TResponse> : IRequestHandler where TRequest : INotification
{
    TResponse Handle(TRequest request, CancellationToken cancellationToken);
}