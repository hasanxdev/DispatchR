using DispatchR.Requests.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTaskWithOutResponse;

public class PingValueTaskWithoutResponseHandler() : IRequestHandler<PingValueTaskWithoutResponse, System.Threading.Tasks.ValueTask>
{
    public System.Threading.Tasks.ValueTask Handle(PingValueTaskWithoutResponse request, CancellationToken cancellationToken)
    {
        return System.Threading.Tasks.ValueTask.CompletedTask;
    }
}