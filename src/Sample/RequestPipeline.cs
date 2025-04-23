using DispatchR;

namespace Sample;

public class RequestPipeline : IRequestPipeline<MyCommand, int>
{
    public Func<MyCommand, CancellationToken, Task<int>> NextPipeline { get; set; }
    public int Priority => 3;
    public Task<int> Handle(MyCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine("RP 1");
        return NextPipeline(command, cancellationToken);
    }
}

public class RequestPipeline2 : IRequestPipeline<MyCommand, int>
{
    public Func<MyCommand, CancellationToken, Task<int>> NextPipeline { get; set; }
    public int Priority => 1;

    public Task<int> Handle(MyCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine("RP 2");
        return NextPipeline(command, cancellationToken);
    }
}