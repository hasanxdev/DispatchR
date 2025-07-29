using DispatchR.TestCommon.Fixtures.SendRequest.Task;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;

namespace DispatchR.TestCommon.Fixtures;

public class Fixture
{
    public static PingTask AnyRequestWithoutPipeline => new();
    public static PingTaskHandler AnyHandlerRequestWithoutPipeline => new();
    
    public static PingValueTask AnyRequestWithPipeline => new();
    public static PingValueTaskHandler AnyHandlerRequestWithPipeline => new();
}