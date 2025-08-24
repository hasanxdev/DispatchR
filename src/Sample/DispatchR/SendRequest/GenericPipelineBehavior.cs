using DispatchR.Abstractions.Send;

namespace Sample.DispatchR.SendRequest;

public class GenericPipelineBehavior<TRequest, TResponse>(ILogger<GenericPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, ValueTask<TResponse>>
    where TRequest : class, IRequest<TRequest, ValueTask<TResponse>>
{
    public ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generic Request Pipeline");
        return NextPipeline.Handle(request, cancellationToken);
    }

    public required IRequestHandler<TRequest, ValueTask<TResponse>> NextPipeline { get; set; }
}