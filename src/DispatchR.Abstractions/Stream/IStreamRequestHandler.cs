using DispatchR.Abstractions.Send;

namespace DispatchR.Abstractions.Stream;

public interface IStreamRequestHandler<TRequest, TResponse> : IRequestHandler where TRequest : class, IStreamRequest
{
    IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}