using DispatchR;

namespace Benchmark;

public sealed record PingDispatchR : IRequest<PingDispatchR, int> { }

public sealed record PingDispatchRWithOutHandler : IRequest<PingDispatchR, int> { }

public sealed class PingHandlerDispatchR : IRequestHandler<PingDispatchR, int>
{
    public Task<int> Handle(PingDispatchR request, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public sealed class LoggingBehaviorDispatchR : IPipelineBehavior<PingDispatchR, int>
{
    public required IRequestHandler<PingDispatchR, int> NextPipeline { get; set; }

    public Task<int> Handle(PingDispatchR request, CancellationToken cancellationToken)
    {
        return NextPipeline.Handle(request, cancellationToken);
    }
}