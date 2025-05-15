using System.Reflection;
using System.Runtime.CompilerServices;
using DispatchR.Requests;
using DispatchR.Requests.Send;
using DispatchR.Requests.Stream;
using Microsoft.Extensions.DependencyInjection;
using ZLinq;


[assembly: ZLinqDropIn("", DropInGenerateTypes.Everything)]

namespace DispatchR;

public static class DispatchRServiceCollection
{
    public static void AddDispatchR(this IServiceCollection services, Assembly assembly, bool withPipelines = true)
    {
        services.AddScoped<IMediator, Mediator>();
        var allTypes = assembly.GetTypes()
            .AsValueEnumerable()
            .Where(p =>
            {
                var interfaces = p.GetInterfaces();
                return interfaces.Length >= 1 &&
                       interfaces.Any(p => p.IsGenericType) &&
                       (interfaces.First(i => i.IsGenericType)
                            .GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                        interfaces.First(i => i.IsGenericType)
                            .GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>) ||
                        interfaces.First(i => i.IsGenericType)
                            .GetGenericTypeDefinition() == typeof(IStreamRequestHandler<,>) ||
                        interfaces.First(i => i.IsGenericType)
                            .GetGenericTypeDefinition() == typeof(IStreamPipelineBehavior<,>));
            });

        var allHandlers = allTypes
            .Where(p =>
            {
                return p.GetInterfaces().First(p => p.IsGenericType)
                    .GetGenericTypeDefinition() == typeof(IRequestHandler<,>);
            }).Concat(allTypes
                .Where(p =>
                {
                    return p.GetInterfaces().First(p => p.IsGenericType)
                        .GetGenericTypeDefinition() == typeof(IStreamRequestHandler<,>);
                }));

        var allPipelines = allTypes
            .Where(p =>
            {
                return p.GetInterfaces().First(p => p.IsGenericType)
                    .GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>);
            });

        var allStreamPipelines = allTypes
            .Where(p =>
            {
                return p.GetInterfaces().First(p => p.IsGenericType)
                    .GetGenericTypeDefinition() == typeof(IStreamPipelineBehavior<,>);
            });

        foreach (var handler in allHandlers)
        {
            var handlerType = typeof(IRequestHandler<,>);
            var behaviorType = typeof(IPipelineBehavior<,>);

            var isStream = handler.GetInterfaces()
                .Any(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IStreamRequestHandler<,>));
            if (isStream)
            {
                handlerType = typeof(IStreamRequestHandler<,>);
                behaviorType = typeof(IStreamPipelineBehavior<,>);
            }

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
                    });

                foreach (var pipeline in pipelines)
                {
                    var interfaceIPipeline = pipeline.GetInterfaces()
                        .First(p => p.GetGenericTypeDefinition() == behaviorType);
                    services.AddScoped(interfaceIPipeline, pipeline);
                }
            }

            services.AddScoped(handler);

            var args = handlerInterface.GetGenericArguments();
            var pipelinesType = behaviorType.MakeGenericType(args[0], args[1]);

            if (isStream)
            {
                services.AddScoped(handlerInterface, sp =>
                {
                    var pipelines = sp
                        .GetServices(pipelinesType)
                        .Select(s => Unsafe.As<IStreamRequestHandler>(s)!);

                    var lastPipeline = Unsafe.As<IStreamRequestHandler>(sp.GetService(handler))!;
                    foreach (var pipeline in pipelines)
                    {
                        pipeline.SetNext(lastPipeline);
                        lastPipeline = pipeline;
                    }

                    return lastPipeline;
                });
            }
            else
            {
                services.AddScoped(handlerInterface, sp =>
                {
                    var pipelines = sp
                        .GetServices(pipelinesType)
                        .Select(s => Unsafe.As<IRequestHandler>(s)!);

                    var lastPipeline = Unsafe.As<IRequestHandler>(sp.GetService(handler))!;
                    foreach (var pipeline in pipelines)
                    {
                        pipeline.SetNext(lastPipeline);
                        lastPipeline = pipeline;
                    }

                    return lastPipeline;
                });
            }
        }
    }
}