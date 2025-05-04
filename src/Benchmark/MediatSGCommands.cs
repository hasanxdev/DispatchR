using Mediator;

namespace Benchmark;

public sealed class PingMediatSG : IRequest<int> { }
public sealed class PingMediatSGWithOutHandler : IRequest<int> { }

public sealed class PingHandlerMediatSG : IRequestHandler<PingMediatSG, int>
{
    public ValueTask<int> Handle(PingMediatSG request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(0);
    }
}

public sealed class LoggingBehaviorMediatSG : IPipelineBehavior<PingMediatSG, int>
{
    // version 2.x
    public ValueTask<int> Handle(PingMediatSG message, CancellationToken cancellationToken, MessageHandlerDelegate<PingMediatSG, int> next)
    {
        return next(message, cancellationToken);
    }

    // version 3.x
    public ValueTask<int> Handle(PingMediatSG message, MessageHandlerDelegate<PingMediatSG, int> next, CancellationToken cancellationToken)
    {
        return next(message, cancellationToken);
    }
}