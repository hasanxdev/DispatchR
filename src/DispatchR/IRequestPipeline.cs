namespace DispatchR;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken t = default);

public interface IRequestPipeline<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
{
    protected Func<TRequest, CancellationToken, Task<TResponse>> NextPipeline { get; set; }
    public virtual int Priority => 1;

    internal void SetNext(Func<TRequest, CancellationToken, Task<TResponse>> handler)
    {
        NextPipeline = handler;
    }
}