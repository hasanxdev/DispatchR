using DispatchR;

namespace Sample;

public class RequestPipeline : IRequestPipeline<MyCommand, int>
{
    public required IRequestHandler<MyCommand, int> NextPipeline { get; set; }
    public Task<int> Handle(MyCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine("RP 1");
        return NextPipeline.Handle(command, cancellationToken);
    }
}

public class RequestPipeline2 : IRequestPipeline<MyCommand, int>
{
    public required IRequestHandler<MyCommand, int> NextPipeline { get; set; }

    public Task<int> Handle(MyCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine("RP 2");
        return NextPipeline.Handle(command, cancellationToken);
    }
}