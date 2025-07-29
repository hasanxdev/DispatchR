using DispatchR.Requests.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ReusedInScopedLifetime;

public class RequestReusedInScopedLifetimeHandler : IRequestHandler<RequestReusedInScopedLifetime, int>
{
    private int _counter = 0;
    public int Handle(RequestReusedInScopedLifetime request, CancellationToken cancellationToken)
    {
        return _counter++;
    }
}