using DispatchR.Requests.Send;

namespace DispatchR.Requests.Stream;

public interface IStreamRequestHandler<TRequest, TResponse> : IRequestHandler where TRequest : class, IStreamRequest, new()
{
    IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}