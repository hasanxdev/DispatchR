using DispatchR.TestCommon.Fixtures.SendRequest.Task;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;
using DispatchR.TestCommon.Fixtures.StreamRequest;

namespace DispatchR.TestCommon.Fixtures;

public class Fixture
{
    public static PingTask AnyRequestWithoutPipeline => new();
    public static PingTaskHandler AnyHandlerRequestWithoutPipeline => new();
    
    public static PingValueTask AnyRequestWithPipeline => new();
    public static PingValueTaskHandler AnyHandlerRequestWithPipeline => new();
    
    public static CounterStreamRequest AnyStreamRequest => new();
    public static CounterStreamHandler AnyStreamHandler => new();
}