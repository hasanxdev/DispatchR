using DispatchR.Abstractions.Notification;
using DispatchR.Abstractions.Send;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR.Configuration
{
    internal static class ServiceRegistrator
    {
        public static void RegisterHandlers(IServiceCollection services, List<Type> allTypes,
            Type requestHandlerType, Type pipelineBehaviorType, Type streamRequestHandlerType,
            Type streamPipelineBehaviorType, bool withPipelines, List<Type>? pipelineOrder = null)
        {
            var handlerTypes = new[] { requestHandlerType, streamRequestHandlerType };
            var pipelineTypes = new[] { pipelineBehaviorType, streamPipelineBehaviorType };

            var allHandlers = allTypes
                .Where(type =>
                {
                    var genericInterfaces = type.GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .Select(i => i.GetGenericTypeDefinition())
                        .ToList();

                    return genericInterfaces.Intersect(handlerTypes).Any() &&
                           !genericInterfaces.Intersect(pipelineTypes).Any();
                })
                .ToList();

            var allPipelines = allTypes
                .Where(type => type.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Select(i => i.GetGenericTypeDefinition())
                    .Intersect(pipelineTypes)
                    .Any())
                .ToList();

            foreach (var handler in allHandlers)
            {
                var handlerInterfaces = handler.GetInterfaces()
                    .Where(p => p.IsGenericType && handlerTypes.Contains(p.GetGenericTypeDefinition()))
                    .ToList();

                foreach (var handlerInterface in handlerInterfaces)
                {
                    object key = Guid.NewGuid();
                    var handlerType = requestHandlerType;
                    var behaviorType = pipelineBehaviorType;

                    var isStream = handlerInterface.GetGenericTypeDefinition() == streamRequestHandlerType;
                    if (isStream)
                    {
                        handlerType = streamRequestHandlerType;
                        behaviorType = streamPipelineBehaviorType;
                    }

                    services.AddKeyedScoped(typeof(IRequestHandler), key, handler);

                    // find pipelines
                    if (withPipelines)
                    {
                        var pipelines = allPipelines
                            .Where(p =>
                            {
                                var interfaces = p.GetInterfaces();
                                if (p.IsGenericType)
                                {
                                    // handle generic pipelines
                                    return interfaces
                                               .FirstOrDefault(inter =>
                                                   inter.IsGenericType &&
                                                   inter.GetGenericTypeDefinition() == behaviorType)
                                               ?.GetInterfaces().First().GetGenericTypeDefinition() ==
                                           handlerInterface.GetGenericTypeDefinition();
                                }

                                return interfaces
                                    .FirstOrDefault(inter =>
                                        inter.IsGenericType &&
                                        inter.GetGenericTypeDefinition() == behaviorType)
                                    ?.GetInterfaces().First() == handlerInterface;
                            }).ToList();

                        // Sort pipelines by the specified order passed via ConfigurationOptions
                        if (pipelineOrder is { Count: > 0 })
                        {
                            pipelines = pipelines
                                .OrderBy(p =>
                                {
                                    var idx = pipelineOrder.IndexOf(p);
                                    return idx == -1 ? int.MaxValue : idx;
                                })
                                .ToList();
                            pipelines.Reverse();
                        }

                        foreach (var pipeline in pipelines)
                        {
                            if (pipeline.IsGenericType)
                            {
                                var genericHandlerResponseType = pipeline.GetInterfaces().First(inter =>
                                    inter.IsGenericType &&
                                    inter.GetGenericTypeDefinition() == behaviorType).GenericTypeArguments[1];

                                var genericHandlerResponseIsAwaitable = IsAwaitable(genericHandlerResponseType);
                                var handlerResponseTypeIsAwaitable = IsAwaitable(handlerInterface.GenericTypeArguments[1]);
                                if (genericHandlerResponseIsAwaitable ^ handlerResponseTypeIsAwaitable)
                                {
                                    continue;
                                }

                                var responseTypeArg = handlerInterface.GenericTypeArguments[1];
                                if (genericHandlerResponseIsAwaitable && handlerResponseTypeIsAwaitable)
                                {
                                    var areGenericTypeArgumentsInHandlerInterfaceMismatched =
                                        genericHandlerResponseType.IsGenericType &&
                                        handlerInterface.GenericTypeArguments[1].IsGenericType &&
                                        genericHandlerResponseType.GetGenericTypeDefinition() !=
                                        handlerInterface.GenericTypeArguments[1].GetGenericTypeDefinition();

                                    if (areGenericTypeArgumentsInHandlerInterfaceMismatched ||
                                        genericHandlerResponseType.IsGenericType ^
                                        handlerInterface.GenericTypeArguments[1].IsGenericType)
                                    {
                                        continue;
                                    }

                                    // If both are non-generic (Task vs ValueTask), then compare directly
                                    if (!responseTypeArg.GenericTypeArguments.Any() &&
                                        !genericHandlerResponseType.GenericTypeArguments.Any() &&
                                        responseTypeArg != genericHandlerResponseType)
                                    {
                                        continue; // Task != ValueTask, so skip
                                    }

                                    // register async generic pipelines
                                    if (responseTypeArg.GenericTypeArguments.Any())
                                    {
                                        responseTypeArg = responseTypeArg.GenericTypeArguments[0];
                                    }
                                }

                                var closedGenericType = pipeline.MakeGenericType(handlerInterface.GenericTypeArguments[0],
                                    responseTypeArg);
                                services.AddKeyedScoped(typeof(IRequestHandler), key, closedGenericType);
                            }
                            else
                            {
                                services.AddKeyedScoped(typeof(IRequestHandler), key, pipeline);
                            }
                        }
                    }

                services.AddScoped(handlerInterface, sp =>
                {
                    var keyedServices = sp.GetKeyedServices<IRequestHandler>(key);

                    var pipelinesWithHandler = keyedServices as IRequestHandler[] ?? keyedServices.ToArray();

                    // Single handler - no pipeline chaining needed
                    if (pipelinesWithHandler.Length == 1)
                    {
                        return pipelinesWithHandler[0];
                    }

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
        }

        public static void RegisterNotification(IServiceCollection services, List<Type> allTypes,
            Type syncNotificationHandlerType)
        {
            var allNotifications = allTypes
                .SelectMany(handlerType => handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && syncNotificationHandlerType == i.GetGenericTypeDefinition())
                    .Select(i => new { HandlerType = handlerType, Interface = i }))
                .ToList();

            foreach (var notification in allNotifications)
            {
                services.AddScoped(notification.Interface, notification.HandlerType);
            }
        }

        private static bool IsAwaitable(Type type)
        {
            if (type == typeof(Task) || type == typeof(ValueTask))
                return true;

            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();
                return genericDef == typeof(Task<>) || genericDef == typeof(ValueTask<>) ||
                       genericDef == typeof(IAsyncEnumerable<>);
            }

            return false;
        }
    }
}