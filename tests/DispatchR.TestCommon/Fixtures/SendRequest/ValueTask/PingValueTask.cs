using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;

public class PingValueTask : IRequest<PingValueTask, ValueTask<int>>
{
    
}