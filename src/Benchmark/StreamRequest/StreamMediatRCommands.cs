using System.Runtime.CompilerServices;
using MediatR;

namespace Benchmark.StreamRequest;

public sealed class PingStreamMediatR : IStreamRequest<int> { }
public sealed class PingStreamMediatRWithOutHandler : IStreamRequest<int> { }

public sealed class PingHandlerMediatR : IStreamRequestHandler<PingStreamMediatR, int>
{
    public async IAsyncEnumerable<int> Handle(PingStreamMediatR request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (int i = 0; i < 3; i++)
        {
            await Task.CompletedTask;
            yield return i;
        }
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