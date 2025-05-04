using DispatchR;

namespace Benchmark;

public sealed record PingDispatchR : IRequest<PingDispatchR, ValueTask<int>> { }

public sealed record PingDispatchRWithOutHandler : IRequest<PingDispatchR, ValueTask<int>> { }

public sealed class PingHandlerDispatchR : IRequestHandler<PingDispatchR, ValueTask<int>>
{
    public ValueTask<int> Handle(PingDispatchR request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(0);
    }
}

public sealed class LoggingBehaviorDispatchR : IPipelineBehavior<PingDispatchR, ValueTask<int>>
{
    public required IRequestHandler<PingDispatchR, ValueTask<int>> NextPipeline { get; set; }

    public ValueTask<int> Handle(PingDispatchR request, CancellationToken cancellationToken)
    {
        return NextPipeline.Handle(request, cancellationToken);
    }
}