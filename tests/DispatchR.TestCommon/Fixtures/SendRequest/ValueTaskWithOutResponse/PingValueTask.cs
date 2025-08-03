using DispatchR.Requests.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTaskWithOutResponse;

public class PingValueTaskWithoutResponse : IRequest<PingValueTaskWithoutResponse, System.Threading.Tasks.ValueTask>
{
}