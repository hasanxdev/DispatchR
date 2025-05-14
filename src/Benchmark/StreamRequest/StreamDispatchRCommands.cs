using System.Runtime.CompilerServices;
using DispatchR.Requests;
using DispatchR.Requests.Stream;

namespace Benchmark.StreamRequest;

public sealed record PingStreamDispatchR : IStreamRequest<PingStreamDispatchR, int> { }

public sealed record PingStreamDispatchRWithOutHandler : IStreamRequest<PingStreamDispatchR, int> { }

public sealed class PingHandlerDispatchR : IStreamRequestHandler<PingStreamDispatchR, int>
{
    public async IAsyncEnumerable<int> Handle(PingStreamDispatchR request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (int i = 0; i < 3; i++)
        {
            await Task.CompletedTask;
            yield return i;
        }
    }
}

public sealed class LoggingBehaviorDispatchR : IStreamPipelineBehavior<PingStreamDispatchR, int>
{
    public required IStreamRequestHandler<PingStreamDispatchR, int> NextPipeline { get; set; }

    public async IAsyncEnumerable<int> Handle(PingStreamDispatchR request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var response in NextPipeline.Handle(request, cancellationToken))
        {
            yield return response;
        }
    }
}