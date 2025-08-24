using System.Runtime.CompilerServices;
using DispatchR.Abstractions.Stream;

namespace Sample.DispatchR.StreamRequest;

public class CounterStreamHandler(IWebHostEnvironment webHostEnvironment) : IStreamRequestHandler<CounterStreamRequest, string>
{
    public async IAsyncEnumerable<string> Handle(CounterStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await using var allLines = File
            .ReadLinesAsync(webHostEnvironment.ContentRootPath + "/BigFile.txt", cancellationToken)
            .GetAsyncEnumerator(cancellationToken);
        
        while (cancellationToken.IsCancellationRequested is false)
        {
            await allLines.MoveNextAsync();
            yield return allLines.Current;
        }
    }
}