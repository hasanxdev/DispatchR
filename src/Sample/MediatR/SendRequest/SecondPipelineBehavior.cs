using MediatR;

namespace Sample.MediatR.SendRequest;

public class SecondPipelineBehavior(ILogger<SecondPipelineBehavior> logger) : IPipelineBehavior<Ping, int>
{
    public async Task<int> Handle(Ping request, RequestHandlerDelegate<int> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Second Request Pipeline");
        return await next(cancellationToken);
    }
}