using DispatchR.Requests.Send;

namespace Sample.DispatchR.SendRequest;

public class GenericPipelineBehavior<TRequest, TResponse>(ILogger<GenericPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TRequest, TResponse>, new()
{
    public TResponse Handle(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generic Request Pipeline");
        return NextPipeline.Handle(request, cancellationToken);
    }

    public required IRequestHandler<TRequest, TResponse> NextPipeline { get; set; }
}