using DispatchR;

namespace Benchmark;

// Request
public sealed class PingDispatchR : IRequest<PingDispatchR, string> { }

// Handler
public sealed class PingHandlerDispatchR : IRequestHandler<PingDispatchR, string>
{
    public Task<string> Handle(PingDispatchR request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Pong");
    }
}

public sealed class LoggingBehaviorDispatchR : IRequestPipeline<PingDispatchR, string>
{
    public IRequestHandler<PingDispatchR, string> NextPipeline { get; set; }

    public Task<string> Handle(PingDispatchR command, CancellationToken cancellationToken)
    {
        return NextPipeline.Handle(command, cancellationToken);
    }
}