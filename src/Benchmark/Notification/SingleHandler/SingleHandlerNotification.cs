using Mediator;

namespace Benchmark.Notification.SingleHandler;

public sealed record SingleHandlerNotification(Guid Id) : INotification, MediatR.INotification, 
    DispatchR.Abstractions.Notification.INotification;