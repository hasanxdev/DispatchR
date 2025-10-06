using DispatchR.Abstractions.Send;
using DispatchR.TestCommon.Fixtures.SendRequest.Task;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;

namespace DispatchR.TestCommon.Fixtures.SendRequest;

public sealed class MultiRequestHandler :
    IRequestHandler<PingTask, Task<int>>,
    IRequestHandler<PingValueTask, ValueTask<int>>
{
    public Task<int> Handle(PingTask request, CancellationToken cancellationToken)
    {
        return System.Threading.Tasks.Task.FromResult(1);
    }

    public ValueTask<int> Handle(PingValueTask request, CancellationToken cancellationToken)
    {
        return System.Threading.Tasks.ValueTask.FromResult(1);
    }
}
