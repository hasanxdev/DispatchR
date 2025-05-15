using System.Runtime.CompilerServices;
using DispatchR.Requests.Send;

namespace DispatchR.Requests.Stream;

public interface IStreamPipelineBehavior<TRequest, TResponse> : IStreamRequestHandler<TRequest, TResponse> 
    where TRequest : class, IStreamRequest<TRequest, TResponse>, new()
{
    public IStreamRequestHandler<TRequest, TResponse> NextPipeline { get; set; }

    void IRequestHandler.SetNext(object handler)
    {
        NextPipeline = Unsafe.As<IStreamRequestHandler<TRequest, TResponse>>(handler);
    }
}