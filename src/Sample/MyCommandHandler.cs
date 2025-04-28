using DispatchR;

namespace Sample;

public class TempRequestHandler : IRequestHandler<MyCommand, int>
{
    public Task<int> Handle(MyCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("request handler");
        return Task.FromResult(1);
    }
}