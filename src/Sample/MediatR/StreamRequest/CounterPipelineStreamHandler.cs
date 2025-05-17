using System.Runtime.CompilerServices;
using MediatR;

namespace Sample.MediatR.StreamRequest;

public class CounterPipelineStreamHandler : IStreamPipelineBehavior<CounterStreamRequest, string>
{
    public async IAsyncEnumerable<string> Handle(CounterStreamRequest request, StreamHandlerDelegate<string> next, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var response in next().WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }
}