using System.Runtime.CompilerServices;
using DispatchR.Requests.Stream;

namespace Sample.DispatchR.StreamRequest;

public class GenericPipelineBehavior<TRequest, TResponse>(ILogger<GenericPipelineBehavior<TRequest, TResponse>> logger)
    : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IStreamRequest<TRequest, TResponse>
{
    public async IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Generic Request Pipeline");
        await foreach (var response in NextPipeline.Handle(request, cancellationToken).ConfigureAwait(false))
        {
            yield return response;
        }
    }

    public IStreamRequestHandler<TRequest, TResponse> NextPipeline { get; set; }
}