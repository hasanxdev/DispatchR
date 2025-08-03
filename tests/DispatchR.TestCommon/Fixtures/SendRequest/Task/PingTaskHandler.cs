using DispatchR.Requests.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.Task;

public class PingTaskHandler() : IRequestHandler<PingTask, Task<int>>
{
    public Task<int> Handle(PingTask request, CancellationToken cancellationToken)
    {
        return System.Threading.Tasks.Task.FromResult(1);
    }
}