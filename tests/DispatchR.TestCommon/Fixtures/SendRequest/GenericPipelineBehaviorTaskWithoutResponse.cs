using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest;

public class GenericPipelineBehaviorTaskWithoutResponse<TRequest, TResponse>()
    : IPipelineBehavior<TRequest, System.Threading.Tasks.Task>
    where TRequest : class, IRequest<TRequest, System.Threading.Tasks.Task>, new()
{
    public System.Threading.Tasks.Task Handle(TRequest request, CancellationToken cancellationToken)
    {
        return NextPipeline.Handle(request, cancellationToken);
    }

    public required IRequestHandler<TRequest, System.Threading.Tasks.Task> NextPipeline { get; set; }
}