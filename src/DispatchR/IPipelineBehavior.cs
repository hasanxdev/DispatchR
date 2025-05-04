namespace DispatchR;

public interface IPipelineBehavior<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
    where TRequest : class, IRequest<TRequest, TResponse>, new()
{
    public IRequestHandler<TRequest, TResponse> NextPipeline { get; set; }

    void IRequestHandler.SetNext(object handler)
    {
        NextPipeline = (IRequestHandler<TRequest, TResponse>)handler;
    }
}