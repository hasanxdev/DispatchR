using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR;

public interface IMediator
{
    Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> command,
        CancellationToken cancellationToken) where TRequest : IRequest;
}

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public Task<TResponse> Send<TRequest, TResponse>(IRequest<TRequest, TResponse> command,
        CancellationToken cancellationToken) where TRequest : IRequest
    {
        var request = (TRequest)command;

        var pipelines = serviceProvider.GetServices<IRequestPipeline<TRequest, TResponse>>().ToList();

        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        
        if (pipelines.Any())
        {
            var handlerWithPipeline = pipelines
                .OrderByDescending(p => p.Priority)
                .ToList();
            
            for (int i = 0; i < pipelines.Count; i++)
            {
                handlerWithPipeline[i].SetNext(i == pipelines.Count - 1
                    ? handler.Handle
                    : handlerWithPipeline[i + 1].Handle);
            }
                
            return handlerWithPipeline.First().Handle(request, cancellationToken);
        }

        return handler.Handle(request, cancellationToken);
    }
}