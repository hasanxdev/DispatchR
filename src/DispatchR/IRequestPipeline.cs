namespace DispatchR;

public interface IRequestPipeline<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
    where TRequest : class, IRequest, new()
{
    public IRequestHandler<TRequest, TResponse> NextPipeline { get; set; }
    void IRequestHandler<TRequest, TResponse>.SetNext(IRequestHandler<TRequest, TResponse> handler) =>
        NextPipeline = handler;
}