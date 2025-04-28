namespace DispatchR;

public interface IRequestHandler<TRequest, TResponse> where TRequest : class, IRequest, new()
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

    internal IRequestHandler<TRequest, TResponse> SetNext(ref IRequestHandler<TRequest, TResponse> handler)
    {
        return this;
    }
}