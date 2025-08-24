using System.Diagnostics.CodeAnalysis;

namespace DispatchR.Abstractions.Send;

public interface IRequestHandler
{
    [ExcludeFromCodeCoverage]
    internal void SetNext(object handler)
    {
    }
}
public interface IRequestHandler<TRequest, TResponse> : IRequestHandler where TRequest : class, IRequest
{
    TResponse Handle(TRequest request, CancellationToken cancellationToken);
}