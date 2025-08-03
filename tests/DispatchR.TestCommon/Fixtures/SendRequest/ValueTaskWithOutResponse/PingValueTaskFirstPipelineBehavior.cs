using DispatchR.Requests.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTaskWithOutResponse;

public class PingValueTaskFirstPipelineBehavior() : IPipelineBehavior<PingValueTaskWithoutResponse, System.Threading.Tasks.ValueTask>
{
    public static DateTime ExecutionTime { get; private set; }
    public required IRequestHandler<PingValueTaskWithoutResponse, System.Threading.Tasks.ValueTask> NextPipeline { get; set; }
    public async System.Threading.Tasks.ValueTask Handle(PingValueTaskWithoutResponse request, CancellationToken cancellationToken)
    {
        ExecutionTime = DateTime.Now;
        await System.Threading.Tasks.Task.Delay(100, cancellationToken).ConfigureAwait(false);
        await NextPipeline.Handle(request, cancellationToken);
    }
}