using System.Runtime.CompilerServices;
using DispatchR.Abstractions.Send;
using DispatchR.TestCommon.Fixtures.Interfaces;

namespace DispatchR.TestCommon.Fixtures.SendRequest;

public class AsyncEnumerablePipelineBehavior<TRequest, TResponse>
    : INonGenericInterface,
    IPipelineBehavior<TRequest, IAsyncEnumerable<TResponse>>
    where TRequest : class, IRequest<TRequest, IAsyncEnumerable<TResponse>>, new()
{
    public required IRequestHandler<TRequest, IAsyncEnumerable<TResponse>> NextPipeline { get; set; }

    public async IAsyncEnumerable<TResponse> Handle(TRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var item in NextPipeline.Handle(request, cancellationToken))
        {
            yield return item;
        }
    }
}
