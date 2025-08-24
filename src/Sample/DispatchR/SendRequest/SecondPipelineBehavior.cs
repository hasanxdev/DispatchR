using DispatchR.Abstractions.Send;

namespace Sample.DispatchR.SendRequest
{
    public class SecondPipelineBehavior(ILogger<SecondPipelineBehavior> logger) : IPipelineBehavior<Ping, ValueTask<int>>
    {
        public required IRequestHandler<Ping, ValueTask<int>> NextPipeline { get; set; }
    
        public ValueTask<int> Handle(Ping request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Second Request Pipeline");
            return NextPipeline.Handle(request, cancellationToken);
        }
    }
}