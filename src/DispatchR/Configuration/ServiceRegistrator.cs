using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using DispatchR.Abstractions.Notification;
using DispatchR.Abstractions.Send;

namespace DispatchR.Configuration
{
    internal static class ServiceRegistrator
    {
        public static void RegisterHandlers(IServiceCollection services, List<Type> allTypes,
            Type requestHandlerType, Type pipelineBehaviorType, Type streamRequestHandlerType,
            Type streamPipelineBehaviorType, bool withPipelines, List<Type>? pipelineOrder = null)
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

        public static void RegisterNotification(IServiceCollection services, List<Type> allTypes,
            Type syncNotificationHandlerType)
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