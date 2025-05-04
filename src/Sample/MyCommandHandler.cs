using DispatchR;

namespace Sample;

public class TempRequestHandler : IRequestHandler<MyCommand, ValueTask<int>>
{
    public ValueTask<int> Handle(MyCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("request handler");
        return ValueTask.FromResult(1);
    }
}