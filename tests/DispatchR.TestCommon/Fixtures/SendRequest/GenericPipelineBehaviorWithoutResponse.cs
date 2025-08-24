using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest;

public class GenericPipelineBehaviorWithOutResponse<TRequest, TResponse>()
    : IPipelineBehavior<TRequest, System.Threading.Tasks.ValueTask>
    where TRequest : class, IRequest<TRequest, System.Threading.Tasks.ValueTask>, new()
{
    public System.Threading.Tasks.ValueTask Handle(TRequest request, CancellationToken cancellationToken)
    {
        return NextPipeline.Handle(request, cancellationToken);
    }

    public required IRequestHandler<TRequest, System.Threading.Tasks.ValueTask> NextPipeline { get; set; }
}