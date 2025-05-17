using MediatR;

namespace Sample.MediatR.SendRequest;

public class FirstPipelineBehavior(ILogger<FirstPipelineBehavior> logger) : IPipelineBehavior<Ping, int>
{
    public async Task<int> Handle(Ping request, RequestHandlerDelegate<int> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("First Request Pipeline");
        return await next(cancellationToken);
    }
}