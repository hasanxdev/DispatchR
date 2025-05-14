using System.Runtime.CompilerServices;
using MediatR;

namespace Sample.MediatR.StreamRequest;

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