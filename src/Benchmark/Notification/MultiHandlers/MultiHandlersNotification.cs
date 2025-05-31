using Mediator;

namespace Benchmark.Notification.MultiHandlers;

public sealed record MultiHandlersNotification(Guid Id) : INotification, MediatR.INotification, 
    DispatchR.Requests.Notification.INotification;