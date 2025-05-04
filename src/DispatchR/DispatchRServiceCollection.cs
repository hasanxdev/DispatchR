using System.Reflection;
using System.Runtime.CompilerServices;
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
                       (interfaces.First(p => p.IsGenericType)
                            .GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                        interfaces.First(p => p.IsGenericType)
                            .GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
            });
        
        var allHandlers = allTypes
            .Where(p =>
            {
                return p.GetInterfaces().First(p => p.IsGenericType)
                           .GetGenericTypeDefinition() == typeof(IRequestHandler<,>);
            });
        
        var allPipelines = allTypes
            .Where(p =>
            {
                return p.GetInterfaces().First(p => p.IsGenericType)
                    .GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>);
            });

        foreach (var handler in allHandlers)
        {
            var handlerInterface = handler.GetInterfaces()
                .First(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

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
                                       inter.GetGenericTypeDefinition() ==
                                       typeof(IPipelineBehavior<,>))
                                   ?.GetInterfaces().First().GetGenericTypeDefinition() ==
                               handlerInterface.GetGenericTypeDefinition();
                    });
                
                foreach (var pipeline in pipelines)
                {
                    var interfaceIPipeline = pipeline.GetInterfaces()
                        .First(p => p.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
                    services.AddScoped(interfaceIPipeline, pipeline);   
                }
            }

            services.AddScoped(handler);
            
            var args = handlerInterface.GetGenericArguments();
            var pipelinesType = typeof(IPipelineBehavior<,>).MakeGenericType(args[0], args[1]);
            services.AddScoped(handlerInterface,   sp =>
            {
                var pipelines = sp
                    .GetServices(pipelinesType)
                    .Select(s => Unsafe.As<IRequestHandler>(s)!);
                
                IRequestHandler lastPipeline = Unsafe.As<IRequestHandler>(sp.GetService(handler))!;
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