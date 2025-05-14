using DispatchR.Requests;

namespace Sample.DispatchR.SendRequest;

public class FirstPipelineBehavior(ILogger<FirstPipelineBehavior> logger) : IPipelineBehavior<Ping, ValueTask<int>>
{
    public required IRequestHandler<Ping, ValueTask<int>> NextPipeline { get; set; }
    public ValueTask<int> Handle(Ping request, CancellationToken cancellationToken)
    {
        logger.LogInformation("First Request Pipeline");
        return NextPipeline.Handle(request, cancellationToken);
    }
}