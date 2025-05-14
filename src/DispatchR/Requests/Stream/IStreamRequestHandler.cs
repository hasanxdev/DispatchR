namespace DispatchR.Requests.Stream;

public interface IStreamRequestHandler
{
    internal void SetNext(object handler)
    {
    }
}
public interface IStreamRequestHandler<TRequest, TResponse> : IStreamRequestHandler where TRequest : class, IStreamRequest, new()
{
    IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}