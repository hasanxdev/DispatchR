using System.Reflection;
using DispatchR.Requests;
using DispatchR.Requests.Notification;
using DispatchR.Requests.Send;
using DispatchR.Requests.Stream;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR;

public static class DispatchRServiceCollection
{
    public static void AddDispatchR(this IServiceCollection services, Assembly assembly, bool withPipelines = true)
    {
        services.AddScoped<IMediator, Mediator>();
        RegisterRequest(services, assembly, withPipelines);
    }

    private static void RegisterRequest(IServiceCollection services, Assembly assembly, bool withPipelines)
    {
        var requestHandlerType = typeof(IRequestHandler<,>);
        var pipelineBehaviorType = typeof(IPipelineBehavior<,>);
        var streamRequestHandlerType = typeof(IStreamRequestHandler<,>);
        var streamPipelineBehaviorType = typeof(IStreamPipelineBehavior<,>);
        var syncNotificationHandlerType = typeof(INotificationHandler<>);
        var aSyncNotificationHandlerType = typeof(INotificationHandler<,>);

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
                           streamPipelineBehaviorType,
                           syncNotificationHandlerType,
                           aSyncNotificationHandlerType
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

        var allNotifications = allTypes
            .Where(p =>
            {
                var @interface = p.GetInterfaces().First(i => i.IsGenericType);
                return new[] { syncNotificationHandlerType, aSyncNotificationHandlerType }
                    .Contains(@interface.GetGenericTypeDefinition());
            }).ToList();

        foreach (var notification in allNotifications)
        {
            var @interface = notification.GetInterfaces()
                .First(i => i.IsGenericType &&
                            new[] { syncNotificationHandlerType, aSyncNotificationHandlerType }
                                .Contains(i.GetGenericTypeDefinition()));
            services.AddScoped(@interface, notification);
        }

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

            services.AddKeyedScoped(typeof(IRequestHandler), key, handler);

            var handlerInterface = handler.GetInterfaces()
                .First(p => p.IsGenericType && p.GetGenericTypeDefinition() == handlerType);

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