
using DispatchR.Requests.Stream;

namespace Sample.DispatchR.StreamRequest;

public class CounterPipelineStreamHandler : IStreamPipelineBehavior<CounterStreamRequest, int>
{
    public IStreamRequestHandler<CounterStreamRequest, int> NextPipeline { get; set; }
    
    public async IAsyncEnumerable<int> Handle(CounterStreamRequest request, CancellationToken cancellationToken)
    {
        await foreach (var response in NextPipeline.Handle(request, cancellationToken))
        {
            yield return response;
        }
    }
}