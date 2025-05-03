using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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
        public static void AddDispatchRV1(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        var openTypes = new[] { typeof(IRequestHandler<,>), typeof(IPipelineBehavior<,>) };
        assembly.GetTypes()
            .AsValueEnumerable()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .AsValueEnumerable()
                .Where(i => i.IsGenericType && openTypes.Contains(i.GetGenericTypeDefinition()))
                .Select(i => new { ServiceType = i, ImplementationType = t })).ToList()
            .ForEach(x => services.AddScoped(x.ServiceType, x.ImplementationType));
    }
    public static void AddDispatchRV2(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        var openTypes = new[] { typeof(IRequestHandler<,>), typeof(IPipelineBehavior<,>) };

        foreach (var type in assembly.GetTypes().AsValueEnumerable())
        {
            if (type.IsClass && !type.IsAbstract)
            {
                foreach (var @interface in type.GetInterfaces().AsValueEnumerable())
                {
                    if (@interface.IsGenericType && openTypes.Contains(@interface.GetGenericTypeDefinition()))
                    {
                        services.AddScoped(@interface, type);
                    }
                }
            }
        }
    }
    public static void AddDispatchRV3(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        var openTypes = new[] { typeof(IRequestHandler<,>), typeof(IPipelineBehavior<,>) };

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsClass && !type.IsAbstract)
            {
                foreach (var @interface in type.GetInterfaces())
                {
                    if (@interface.IsGenericType && openTypes.Contains(@interface.GetGenericTypeDefinition()))
                    {
                        services.AddScoped(@interface, type);
                    }
                }
            }
        }
    }
}
