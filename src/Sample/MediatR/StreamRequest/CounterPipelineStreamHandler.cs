using MediatR;

namespace Sample.MediatR.StreamRequest;

public class CounterPipelineStreamHandler : IStreamPipelineBehavior<CounterStreamRequest, int>
{
    public async IAsyncEnumerable<int> Handle(CounterStreamRequest request, StreamHandlerDelegate<int> next, CancellationToken cancellationToken)
    {
        await foreach (var response in next().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }
}