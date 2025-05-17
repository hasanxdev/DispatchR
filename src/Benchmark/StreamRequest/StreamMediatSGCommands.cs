#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
using System.Runtime.CompilerServices;
using Mediator;

namespace Benchmark.StreamRequest;

public sealed class PingStreamMediatSg : IStreamRequest<int> { }
public sealed class PingStreamMediatSgWithOutHandler : IStreamRequest<int> { }

public sealed class PingHandlerMediatSg : IStreamRequestHandler<PingStreamMediatSg, int>
{
    public async IAsyncEnumerable<int> Handle(PingStreamMediatSg request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return 0;
    }
}

public sealed class LoggingBehaviorMediatSg : IStreamPipelineBehavior<PingStreamMediatSg, int>
{
    // version 2.x
    public async IAsyncEnumerable<int> Handle(PingStreamMediatSg message, [EnumeratorCancellation] CancellationToken cancellationToken, StreamHandlerDelegate<PingStreamMediatSg, int> next)
    {
        await foreach (var response in next(message, cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }
}