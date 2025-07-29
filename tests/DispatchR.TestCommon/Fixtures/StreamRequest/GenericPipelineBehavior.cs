using DispatchR.Requests.Stream;

namespace DispatchR.TestCommon.Fixtures.StreamRequest;

public class GenericPipelineBehavior<TRequest, TResponse>()
    : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IStreamRequest<TRequest, TResponse>, new()
{
    public async IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        await foreach (var response in NextPipeline.Handle(request, cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }

    public IStreamRequestHandler<TRequest, TResponse> NextPipeline { get; set; }
}