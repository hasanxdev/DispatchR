#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
using System.Runtime.CompilerServices;
using DispatchR.Abstractions.Stream;

namespace Benchmark.StreamRequest;

public sealed record PingStreamDispatchR : IStreamRequest<PingStreamDispatchR, int> { }

public sealed record PingStreamDispatchRWithOutHandler : IStreamRequest<PingStreamDispatchR, int> { }

public sealed class PingHandlerDispatchR : IStreamRequestHandler<PingStreamDispatchR, int>
{
    public async IAsyncEnumerable<int> Handle(PingStreamDispatchR request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return 0;
    }
}

public sealed class LoggingBehaviorDispatchR : IStreamPipelineBehavior<PingStreamDispatchR, int>
{
    public required IStreamRequestHandler<PingStreamDispatchR, int> NextPipeline { get; set; }

    public async IAsyncEnumerable<int> Handle(PingStreamDispatchR request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var response in NextPipeline.Handle(request, cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }
}