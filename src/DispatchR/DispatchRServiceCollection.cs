using System.Reflection;
using System.Runtime.CompilerServices;
using DispatchR.Requests;
using DispatchR.Requests.Notification;
using DispatchR.Requests.Send;
using DispatchR.Requests.Stream;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR;

public static class DispatchRServiceCollection
{
    public static void AddDispatchR(this IServiceCollection services, Assembly assembly, bool withPipelines = true, bool withNotifications = true)
    {
        services.AddScoped<IMediator, Mediator>();
        var requestHandlerType = typeof(IRequestHandler<,>);
        var pipelineBehaviorType = typeof(IPipelineBehavior<,>);
        var streamRequestHandlerType = typeof(IStreamRequestHandler<,>);
        var streamPipelineBehaviorType = typeof(IStreamPipelineBehavior<,>);
        var syncNotificationHandlerType = typeof(INotificationHandler<>);

        var allTypes = assembly.GetTypes()
            .Where(p =>
            {
                var interfaces = p.GetInterfaces();
                return interfaces.Length >= 1 &&
                       interfaces
                           .Where(i => i.IsGenericType)
                           .Select(i => i.GetGenericTypeDefinition())
                           .Any(i => new[]
                           {
                               requestHandlerType,
                               pipelineBehaviorType,
                               streamRequestHandlerType,
                               streamPipelineBehaviorType,
                               syncNotificationHandlerType
                           }.Contains(i));
            }).ToList();

        if (withNotifications)
        {
            RegisterNotification(services, allTypes, syncNotificationHandlerType);
        }
        
        RegisterHandlers(services, allTypes, requestHandlerType, pipelineBehaviorType, 
            streamRequestHandlerType, streamPipelineBehaviorType, withPipelines);
    }

    private static void RegisterHandlers(IServiceCollection services, List<Type> allTypes,
        Type requestHandlerType, Type pipelineBehaviorType, Type streamRequestHandlerType,
        Type streamPipelineBehaviorType, bool withPipelines)
    {
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
                var pipelinesWithHandler = Unsafe
                    .As<IRequestHandler[]>(sp.GetKeyedServices<IRequestHandler>(key));
                
                IRequestHandler lastPipeline = pipelinesWithHandler[0];
                for (int i = 1; i < pipelinesWithHandler.Length; i++)
                {
                    var pipeline = pipelinesWithHandler[i];
                    pipeline.SetNext(lastPipeline);
                    lastPipeline = pipeline;
                }

                return lastPipeline;
            });
        }
    }

    private static void RegisterNotification(IServiceCollection services, List<Type> allTypes, Type syncNotificationHandlerType)
    {
        var allNotifications = allTypes
            .Where(p =>
            {
                return p.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Select(i => i.GetGenericTypeDefinition())
                    .Any(i => new[]
                    {
                        syncNotificationHandlerType
                    }.Contains(i));
            })
            .GroupBy(p =>
            {
                var @interface = p.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .First(i => new[]
                    {
                        syncNotificationHandlerType
                    }.Contains(i.GetGenericTypeDefinition()));
                return @interface.GenericTypeArguments.First();
            })
            .ToList();

        foreach (var notification in allNotifications)
        {
            foreach (var types in notification.ToList())
            {
                services.AddScoped(typeof(INotificationHandler<>).MakeGenericType(notification.Key), types);
            }
        }
    }
}