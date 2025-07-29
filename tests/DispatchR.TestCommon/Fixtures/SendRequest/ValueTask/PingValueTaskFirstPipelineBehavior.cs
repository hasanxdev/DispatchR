using DispatchR.Requests.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;

public class PingValueTaskFirstPipelineBehavior() : IPipelineBehavior<PingValueTask, ValueTask<int>>
{
    public static DateTime ExecutionTime { get; private set; }
    public required IRequestHandler<PingValueTask, ValueTask<int>> NextPipeline { get; set; }
    public ValueTask<int> Handle(PingValueTask request, CancellationToken cancellationToken)
    {
        ExecutionTime = DateTime.Now;
        return NextPipeline.Handle(request, cancellationToken);
    }
}