using MediatR;

namespace Sample.MediatR.SendRequest;

public class GenericPipelineBehavior<TRequest, TResponse>(ILogger<GenericPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generic Request Pipeline");
        return await next(cancellationToken);
    }
}