using System.Runtime.CompilerServices;

namespace DispatchR.Requests.Stream;

public interface IStreamPipelineBehavior<TRequest, TResponse> : IStreamRequestHandler<TRequest, TResponse> 
    where TRequest : class, IStreamRequest<TRequest, TResponse>, new()
{
    public IStreamRequestHandler<TRequest, TResponse> NextPipeline { get; set; }

    void IStreamRequestHandler.SetNext(object handler)
    {
        NextPipeline = Unsafe.As<IStreamRequestHandler<TRequest, TResponse>>(handler);
    }
}