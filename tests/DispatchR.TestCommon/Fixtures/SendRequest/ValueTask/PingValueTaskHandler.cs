using DispatchR.Requests.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;

public class PingValueTaskHandler() : IRequestHandler<PingValueTask, ValueTask<int>>
{
    public ValueTask<int> Handle(PingValueTask request, CancellationToken cancellationToken)
    {
        return System.Threading.Tasks.ValueTask.FromResult(1);
    }
}