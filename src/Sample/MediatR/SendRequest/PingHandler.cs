using MediatR;

namespace Sample.MediatR.SendRequest;

public class PingHandler(ILogger<PingHandler> logger) : IRequestHandler<Ping, int>
{
    public Task<int> Handle(Ping request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received in request handler");
        return Task.FromResult(1);
    }
}