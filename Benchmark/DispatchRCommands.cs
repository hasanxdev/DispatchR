using DispatchR;

namespace Benchmark;

// Request
public class PingDispatchR : IRequest<PingDispatchR, string> { }

// Handler
public class PingHandlerDispatchR : IRequestHandler<PingDispatchR, string>
{
    public Task<string> Handle(PingDispatchR request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Pong");
    }
}

public class LoggingBehaviorDispatchR : IRequestPipeline<PingDispatchR, string>
{
    public Task<string> Handle(PingDispatchR command, CancellationToken cancellationToken)
    {
        return NextPipeline(command, cancellationToken);
    }

    public Func<PingDispatchR, CancellationToken, Task<string>> NextPipeline { get; set; }
}