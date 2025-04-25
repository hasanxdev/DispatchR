namespace DispatchR;

public interface IRequestHandler<TRequest, TResponse> where TRequest : class, IRequest, new()
{
    Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken);

    internal void SetNext(IRequestHandler<TRequest, TResponse> handler)
    {
    }
}