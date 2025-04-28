namespace DispatchR;

public interface IPipelineBehavior<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
    where TRequest : class, IRequest<TRequest, TResponse>, new()
{
    public IRequestHandler<TRequest, TResponse> NextPipeline { get; set; }

    IRequestHandler<TRequest, TResponse> IRequestHandler<TRequest, TResponse>.SetNext(ref IRequestHandler<TRequest, TResponse> handler)
    {
        NextPipeline = handler;
        return this;
    }
}