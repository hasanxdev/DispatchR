using System.Reflection;
using DispatchR.Requests;
using DispatchR.Requests.Send;
using DispatchR.Requests.Stream;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR;

public static class DispatchRServiceCollection
{
    public static void AddDispatchR(this IServiceCollection services, Assembly assembly, bool withPipelines = true)
    {
        services.AddScoped<IMediator, Mediator>();
        var requestHandlerType = typeof(IRequestHandler<,>);
        var pipelineBehaviorType = typeof(IPipelineBehavior<,>);
        var streamRequestHandlerType = typeof(IStreamRequestHandler<,>);
        var streamPipelineBehaviorType = typeof(IStreamPipelineBehavior<,>);

        var allTypes = assembly.GetTypes()
            .Where(p =>
            {
                var interfaces = p.GetInterfaces();
                return interfaces.Length >= 1 &&
                       interfaces.Any(i => i.IsGenericType) &&
                       new[]
                       {
                           requestHandlerType,
                           pipelineBehaviorType,
                           streamRequestHandlerType,
                           streamPipelineBehaviorType
                       }.Contains(interfaces.First(i => i.IsGenericType).GetGenericTypeDefinition());
            }).ToList();

        var allHandlers = allTypes
            .Where(p =>
            {
                var @interface = p.GetInterfaces().First(i => i.IsGenericType);
                return new[] { requestHandlerType, streamRequestHandlerType }
                    .Contains(@interface.GetGenericTypeDefinition());
            }).ToList();

        var allPipelines = allTypes
            .Where(p =>
            {
                var @interface = p.GetInterfaces().First(i => i.IsGenericType);
                return new[] { pipelineBehaviorType, streamPipelineBehaviorType }
                    .Contains(@interface.GetGenericTypeDefinition());
            }).ToList();

        foreach (var handler in allHandlers)
        {
            object key = handler.GUID;
            var handlerType = requestHandlerType;
            var behaviorType = pipelineBehaviorType;

            var isStream = handler.GetInterfaces()
                .Any(p => p.IsGenericType && p.GetGenericTypeDefinition() == streamRequestHandlerType);
            if (isStream)
            {
                handlerType = streamRequestHandlerType;
                behaviorType = streamPipelineBehaviorType;
            }

            var handlerInterface = handler.GetInterfaces()
                .First(p => p.IsGenericType && p.GetGenericTypeDefinition() == handlerType);

            services.AddKeyedScoped(typeof(IRequestHandler), key, handler);

            // find pipelines
            if (withPipelines)
            {
                var pipelines = allPipelines
                    .Where(p =>
                    {
                        var interfaces = p.GetInterfaces();
                        return interfaces
                                   .FirstOrDefault(inter =>
                                       inter.IsGenericType &&
                                       inter.GetGenericTypeDefinition() == behaviorType)
                                   ?.GetInterfaces().First().GetGenericTypeDefinition() ==
                               handlerInterface.GetGenericTypeDefinition();
                    }).ToList();

                foreach (var pipeline in pipelines)
                {
                    services.AddKeyedScoped(typeof(IRequestHandler), key, pipeline);
                }
            }

            services.AddScoped(handlerInterface, sp =>
            {
                using var pipelinesWithHandler = sp
                    .GetKeyedServices<IRequestHandler>(key)
                    .GetEnumerator();
                
                IRequestHandler? lastPipeline = null;
                while (pipelinesWithHandler.MoveNext())
                {
                    var pipeline = pipelinesWithHandler.Current;
                    pipeline.SetNext(lastPipeline!);
                    lastPipeline = pipeline;
                }

                return lastPipeline!;
            });
        }
    }
}