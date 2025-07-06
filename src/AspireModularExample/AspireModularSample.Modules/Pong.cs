using DispatchR.Requests.Send;

namespace AspireModularSample.Modules
{
    public class Pong : IRequest<Pong, ValueTask<string>>
    {
    }

    public class PongHandler : IRequestHandler<Pong, ValueTask<string>>
    {
        public ValueTask<string> Handle(Pong request, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult("Pong from ServiceB");
        }
    }
}