using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.Sync;

public class PingHandler() : IRequestHandler<Ping, int>
{
    public int Handle(Ping request, CancellationToken cancellationToken)
    {
        return 1;
    }
}