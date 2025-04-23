using MediatR;

namespace Benchmark;

// Request
public class PingMediatR : IRequest<string> { }

// Handler
public class PingHandlerMediatR : IRequestHandler<PingMediatR, string>
{
    public Task<string> Handle(PingMediatR request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Pong");
    }
}


public class LoggingBehaviorMediat : IPipelineBehavior<PingMediatR, string>
{
    public Task<string> Handle(PingMediatR request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        // Console.WriteLine("MediatR Logging Behavior");
        return next(cancellationToken);
    }
}