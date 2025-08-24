using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest;

public class GenericPipelineBehaviorWithResponse<TRequest, TResponse>()
    : IPipelineBehavior<TRequest, ValueTask<TResponse>>
    where TRequest : class, IRequest<TRequest, ValueTask<TResponse>>, new()
{
    public ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        return NextPipeline.Handle(request, cancellationToken);
    }

    public required IRequestHandler<TRequest, ValueTask<TResponse>> NextPipeline { get; set; }
}