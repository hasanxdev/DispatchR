using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTaskWithOutResponse;

public class PingValueTaskWithoutResponse : IRequest<PingValueTaskWithoutResponse, System.Threading.Tasks.ValueTask>
{
}