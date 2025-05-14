using System.Runtime.CompilerServices;
using DispatchR.Requests.Stream;

namespace Sample.DispatchR.StreamRequest;

public class CounterStreamHandler : IStreamRequestHandler<CounterStreamRequest, int>
{
    public async IAsyncEnumerable<int> Handle(CounterStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        int count = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.CompletedTask;
            yield return count;
            count++;
        }
    }
}