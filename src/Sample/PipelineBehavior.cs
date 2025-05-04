using DispatchR;

namespace Sample;

public class PipelineBehavior : IPipelineBehavior<MyCommand, ValueTask<int>>
{
    public required IRequestHandler<MyCommand, ValueTask<int>> NextPipeline { get; set; }
    public ValueTask<int> Handle(MyCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("RP 1");
        return NextPipeline.Handle(request, cancellationToken);
    }

    // public async Task<int> Handle(MyCommand request, RequestHandlerDelegate<int> next, CancellationToken cancellationToken)
    // {
    //     Console.WriteLine("RP 1");
    //     return await next(cancellationToken);
    // }
}

public class Pipeline2 : IPipelineBehavior<MyCommand, ValueTask<int>>
{
    public required IRequestHandler<MyCommand, ValueTask<int>> NextPipeline { get; set; }
    
    public ValueTask<int> Handle(MyCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("RP 2");
        return NextPipeline.Handle(request, cancellationToken);
    }

    // public async Task<int> Handle(MyCommand request, RequestHandlerDelegate<int> next, CancellationToken cancellationToken)
    // {
    //     Console.WriteLine("RP 2");
    //     return await next(cancellationToken);
    // }
}