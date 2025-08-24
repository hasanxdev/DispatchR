using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.Task;

public class PingTask : IRequest<PingTask, Task<int>>
{
    
}