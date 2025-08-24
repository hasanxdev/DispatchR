using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.ValueTaskWithOutResponse
{
    public class PingValueTaskWithoutResponseSecondPipelineBehavior() : IPipelineBehavior<PingValueTaskWithoutResponse, System.Threading.Tasks.ValueTask>
    {
        public static DateTime ExecutionTime { get; private set; }
        public required IRequestHandler<PingValueTaskWithoutResponse, System.Threading.Tasks.ValueTask> NextPipeline { get; set; }
    
        public System.Threading.Tasks.ValueTask Handle(PingValueTaskWithoutResponse request, CancellationToken cancellationToken)
        {
            ExecutionTime = DateTime.Now;
            return NextPipeline.Handle(request, cancellationToken);
        }
    }
}