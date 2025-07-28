using DispatchR.Requests.Send;

namespace DispatchR.UnitTest.Fixtures.SendRequest;

public class PingValueTaskHandler() : IRequestHandler<PingValueTask, ValueTask<int>>
{
    public ValueTask<int> Handle(PingValueTask request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(1);
    }
}