using System.Runtime.CompilerServices;
using DispatchR.Abstractions.Send;

namespace DispatchR.Abstractions.Stream;

public interface IStreamPipelineBehavior<TRequest, TResponse> : IStreamRequestHandler<TRequest, TResponse> 
    where TRequest : class, IStreamRequest<TRequest, TResponse>
{
    public IStreamRequestHandler<TRequest, TResponse> NextPipeline { get; set; }

    void IRequestHandler.SetNext(object handler)
    {
        NextPipeline = Unsafe.As<IStreamRequestHandler<TRequest, TResponse>>(handler);
    }
}