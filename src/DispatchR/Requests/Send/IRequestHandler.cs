namespace DispatchR.Requests.Send;

public interface IRequestHandler
{
    internal void SetNext(object handler)
    {
    }
}
public interface IRequestHandler<TRequest, TResponse> : IRequestHandler where TRequest : class, IRequest
{
    TResponse Handle(TRequest request, CancellationToken cancellationToken);
}