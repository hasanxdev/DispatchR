using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace DispatchR;

public static class DispatchRServiceCollection
{
    public static void AddDispatchRHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        assembly.GetTypes()
            .Where(p => p.GetInterfaces().Length >= 1 &&
                        p.GetInterfaces().Any(p => p.IsGenericType) &&
                        p.GetInterfaces().First(p => p.IsGenericType).GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
            )
            .ToList()
            .ForEach(handler =>
            {
                var requestInterface = handler.GetInterfaces()
                    .First(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
                services.AddScoped(requestInterface, handler);
            });
    }
    
    public static void AddDispatchR(this IServiceCollection services, Assembly assembly)
    {
        services.AddDispatchRHandlers(assembly);

        assembly.GetTypes()
            .Where(p => p.GetInterfaces().Length >= 1 &&
                        p.GetInterfaces().Any(p => p.IsGenericType) &&
                        p.GetInterfaces().First(p => p.IsGenericType).GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)
            )
            .ToList()
            .ForEach(handler =>
            {
                var requestInterface = handler.GetInterfaces()
                    .First(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
                services.AddScoped(requestInterface.GetInterfaces().First(), handler);
            });
    }
}