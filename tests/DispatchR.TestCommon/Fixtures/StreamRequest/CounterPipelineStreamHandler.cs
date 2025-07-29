using System.Runtime.CompilerServices;
using DispatchR.Requests.Stream;

namespace DispatchR.TestCommon.Fixtures.StreamRequest;

public class CounterPipelineStreamHandler : IStreamPipelineBehavior<CounterStreamRequest, string>
{
    public required IStreamRequestHandler<CounterStreamRequest, string> NextPipeline { get; set; }
    
    public async IAsyncEnumerable<string> Handle(CounterStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var response in NextPipeline.Handle(request, cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }
}