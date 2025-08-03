using DispatchR.TestCommon.Fixtures.SendRequest.Task;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTaskWithOutResponse;
using DispatchR.TestCommon.Fixtures.StreamRequest;

namespace DispatchR.TestCommon.Fixtures;

public class Fixture
{
    public static PingTaskHandler AnyHandlerRequestWithoutPipeline => new();
    
    public static PingValueTaskHandler AnyHandlerRequestWithPipeline => new();
    
    public static PingValueTaskWithoutResponse AnyRequestWithoutResponsePipeline => new();
    public static PingValueTaskWithoutResponseHandler AnyHandlerRequestWithoutResponseWithPipeline => new();
    
    public static CounterStreamRequest AnyStreamRequest => new();
    public static CounterStreamHandler AnyStreamHandler => new();
}