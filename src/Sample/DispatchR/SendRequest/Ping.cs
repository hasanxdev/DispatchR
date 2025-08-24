using DispatchR.Abstractions.Send;

namespace Sample.DispatchR.SendRequest
{
    public class Ping : IRequest<Ping, ValueTask<int>>
    {
    
    }
}