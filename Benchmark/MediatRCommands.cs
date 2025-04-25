using MediatR;

namespace Benchmark;

// Request
public sealed class PingMediatR : IRequest<string> { }

// Handler
public sealed class PingHandlerMediatR : IRequestHandler<PingMediatR, string>
{
    public Task<string> Handle(PingMediatR request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Pong");
    }
}


public sealed class LoggingBehaviorMediat : IPipelineBehavior<PingMediatR, string>
{
    public Task<string> Handle(PingMediatR request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        // Console.WriteLine("MediatR Logging Behavior");
        return next(cancellationToken);
    }
}