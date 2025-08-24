using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;

public class PingValueTaskFirstPipelineBehavior() : IPipelineBehavior<PingValueTask, ValueTask<int>>
{
    public static DateTime ExecutionTime { get; private set; }
    public required IRequestHandler<PingValueTask, ValueTask<int>> NextPipeline { get; set; }
    public async ValueTask<int> Handle(PingValueTask request, CancellationToken cancellationToken)
    {
        ExecutionTime = DateTime.Now;
        await System.Threading.Tasks.Task.Delay(100, cancellationToken).ConfigureAwait(false);
        return await NextPipeline.Handle(request, cancellationToken);
    }
}