namespace DispatchR;

public interface IRequestHandler
{
    internal void SetNext(object handler)
    {
    }
}
public interface IRequestHandler<TRequest, TResponse> : IRequestHandler where TRequest : class, IRequest, new()
{
    TResponse Handle(TRequest request, CancellationToken cancellationToken);
}