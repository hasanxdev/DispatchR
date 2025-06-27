using DispatchR.Configuration;
using DispatchR.Requests;
using DispatchR.Requests.Notification;
using DispatchR.Requests.Send;
using DispatchR.Requests.Stream;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DispatchR.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDispatchR(this IServiceCollection services, Action<ConfigurationOptions> configuration)
    {
        var config = new ConfigurationOptions();
        configuration(config);

        return services.AddDispatchR(config);
    }

    public static IServiceCollection AddDispatchR(this IServiceCollection services, Assembly assembly, bool withPipelines = true, bool withNotifications = true)
    {
        var config = new ConfigurationOptions
        {
            RegisterPipelines = withPipelines,
            RegisterNotifications = withNotifications
        };

        config.Assemblies.Add(assembly);

        return services.AddDispatchR(config);
    }

    public static IServiceCollection AddDispatchR(this IServiceCollection services, ConfigurationOptions configurationOptions)
    {
        services.AddScoped<IMediator, Mediator>();
        var requestHandlerType = typeof(IRequestHandler<,>);
        var pipelineBehaviorType = typeof(IPipelineBehavior<,>);
        var streamRequestHandlerType = typeof(IStreamRequestHandler<,>);
        var streamPipelineBehaviorType = typeof(IStreamPipelineBehavior<,>);
        var syncNotificationHandlerType = typeof(INotificationHandler<>);

        var allTypes = configurationOptions.Assemblies.SelectMany(x => x.GetTypes())
            .Distinct()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type =>
            {
                var interfaces = type.GetInterfaces();
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

        if (configurationOptions.RegisterNotifications)
        {
            ServiceRegistrator.RegisterNotification(services, allTypes, syncNotificationHandlerType);
        }

        ServiceRegistrator.RegisterHandlers(services, allTypes, requestHandlerType, pipelineBehaviorType,
            streamRequestHandlerType, streamPipelineBehaviorType, configurationOptions.RegisterPipelines, configurationOptions.PipelineOrder);

        return services;
    }
}