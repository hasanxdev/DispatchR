using System.Runtime.CompilerServices;
using DispatchR.Requests.Stream;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR.Requests;

public interface IMediator
{
    TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new();

    IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IStreamRequest, new();
}

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public TResponse Send<TRequest, TResponse>(IRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken) where TRequest : class, IRequest, new()
    {
        return serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .Handle(Unsafe.As<TRequest>(request), cancellationToken);
    }

    public IAsyncEnumerable<TResponse> CreateStream<TRequest, TResponse>(IStreamRequest<TRequest, TResponse> request, CancellationToken cancellationToken) where TRequest : class, IStreamRequest, new()
    {
        return serviceProvider.GetRequiredService<IStreamRequestHandler<TRequest, TResponse>>()
            .Handle(Unsafe.As<TRequest>(request), cancellationToken);
    }
}