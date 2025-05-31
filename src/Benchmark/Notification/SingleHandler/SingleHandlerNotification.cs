using Mediator;

namespace Benchmark.Notification.SingleHandler;

public sealed record SingleHandlerNotification(Guid Id) : INotification, MediatR.INotification, 
    DispatchR.Requests.Notification.INotification;