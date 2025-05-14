using System.Runtime.CompilerServices;
using Mediator;

namespace Benchmark.StreamRequest;

public sealed class PingStreamMediatSg : IStreamRequest<int> { }
public sealed class PingStreamMediatSgWithOutHandler : IStreamRequest<int> { }

public sealed class PingHandlerMediatSg : IStreamRequestHandler<PingStreamMediatSg, int>
{
    public async IAsyncEnumerable<int> Handle(PingStreamMediatSg request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (int i = 0; i < 3; i++)
        {
            await Task.CompletedTask;
            yield return i;
        }
    }
}

public sealed class LoggingBehaviorMediatSg : IStreamPipelineBehavior<PingStreamMediatSg, int>
{
    // version 2.x
    public async IAsyncEnumerable<int> Handle(PingStreamMediatSg message, [EnumeratorCancellation] CancellationToken cancellationToken, StreamHandlerDelegate<PingStreamMediatSg, int> next)
    {
        await foreach (var response in next(message, cancellationToken))
        {
            yield return response;
        }
    }
}