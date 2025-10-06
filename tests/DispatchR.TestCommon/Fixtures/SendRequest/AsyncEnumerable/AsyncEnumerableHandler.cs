using System.Runtime.CompilerServices;
using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.AsyncEnumerable;

public class AsyncEnumerableHandler : IRequestHandler<AsyncEnumerableRequest, IAsyncEnumerable<int>>
{
    public async IAsyncEnumerable<int> Handle(AsyncEnumerableRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return await System.Threading.Tasks.Task.FromResult(1);
        yield return await System.Threading.Tasks.Task.FromResult(2);
        yield return await System.Threading.Tasks.Task.FromResult(3);
    }
}
