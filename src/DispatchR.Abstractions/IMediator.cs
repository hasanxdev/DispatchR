using DispatchR.Abstractions.Notification;
using DispatchR.Abstractions.Send;
using DispatchR.Abstractions.Stream;

namespace DispatchR;

public interface IMediator
{
    TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest;

    IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IStreamRequest;

    ValueTask Publish<TNotification>(TNotification request, CancellationToken cancellationToken)
        where TNotification : INotification;
    
    /// <summary>
    /// This method is not recommended for performance-critical scenarios.  
    /// Use it only if it is strictly necessary, as its performance is lower compared  
    /// to similar methods in terms of both memory usage and CPU consumption.  
    /// </summary>
    /// <param name="request">
    /// An object that implements INotification
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete(message: "This method has performance issues. Use only if strictly necessary", error: false)]
    ValueTask Publish(object request, CancellationToken cancellationToken);
}
