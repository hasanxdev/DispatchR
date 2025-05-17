#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
using System.Runtime.CompilerServices;
using MediatR;
namespace Benchmark.StreamRequest;

public sealed class PingStreamMediatR : IStreamRequest<int> { }
public sealed class PingStreamMediatRWithOutHandler : IStreamRequest<int> { }

public sealed class PingHandlerMediatR : IStreamRequestHandler<PingStreamMediatR, int>
{
    public async IAsyncEnumerable<int> Handle(PingStreamMediatR request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return 0;
    }
}

public sealed class LoggingBehaviorMediat : IStreamPipelineBehavior<PingStreamMediatR, int>
{
    public async IAsyncEnumerable<int> Handle(PingStreamMediatR request, StreamHandlerDelegate<int> next, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var response in next().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }
}