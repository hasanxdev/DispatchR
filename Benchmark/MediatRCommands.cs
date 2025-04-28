using MediatR;

namespace Benchmark;

public sealed class PingMediatR : IRequest<int> { }
public sealed class PingMediatRWithOutHandler : IRequest<int> { }

public sealed class PingHandlerMediatR : IRequestHandler<PingMediatR, int>
{
    public Task<int> Handle(PingMediatR request, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

public sealed class LoggingBehaviorMediat : IPipelineBehavior<PingMediatR, int>
{
    public Task<int> Handle(PingMediatR request, RequestHandlerDelegate<int> next, CancellationToken cancellationToken)
    {
        return next(cancellationToken);
    }
}