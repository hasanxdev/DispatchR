using DispatchR.Requests;
using DispatchR.Requests.Send;

namespace Sample.DispatchR.SendRequest
{
    public class PingHandler(ILogger<PingHandler> logger) : IRequestHandler<Ping, ValueTask<int>>
    {
        public ValueTask<int> Handle(Ping request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Received in request handler");
            return ValueTask.FromResult(1);
        }
    }
}