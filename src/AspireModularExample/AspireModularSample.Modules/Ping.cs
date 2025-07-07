using DispatchR.Requests.Send;

namespace AspireModularSample.Modules
{
    public class Ping : IRequest<Ping, ValueTask<string>>
    {
    }

    public class PingHandler : IRequestHandler<Ping, ValueTask<string>>
    {
        public ValueTask<string> Handle(Ping request, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult("Ping from ServiceA");
        }
    }
}