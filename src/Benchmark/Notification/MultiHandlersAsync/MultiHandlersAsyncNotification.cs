using Mediator;

namespace Benchmark.Notification.MultiHandlersAsync;

public sealed record MultiHandlersAsyncNotification(Guid Id) : INotification, MediatR.INotification, 
    DispatchR.Requests.Notification.INotification;