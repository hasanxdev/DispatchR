using DispatchR.Requests.Send;

namespace DispatchR.UnitTest.Fixtures.SendRequest
{
    public class SecondPipelineBehavior() : IPipelineBehavior<PingValueTask, ValueTask<int>>
    {
        public required IRequestHandler<PingValueTask, ValueTask<int>> NextPipeline { get; set; }
    
        public ValueTask<int> Handle(PingValueTask request, CancellationToken cancellationToken)
        {
            return NextPipeline.Handle(request, cancellationToken);
        }
    }
}