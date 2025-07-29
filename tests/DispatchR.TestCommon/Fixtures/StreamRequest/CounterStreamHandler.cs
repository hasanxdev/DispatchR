using System.Runtime.CompilerServices;
using DispatchR.Requests.Stream;

namespace DispatchR.TestCommon.Fixtures.StreamRequest;

public class CounterStreamHandler() : IStreamRequestHandler<CounterStreamRequest, string>
{
    public async IAsyncEnumerable<string> Handle(CounterStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        yield return string.Empty;
    }
}