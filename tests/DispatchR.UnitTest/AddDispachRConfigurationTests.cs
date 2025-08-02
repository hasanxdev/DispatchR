using DispatchR.Exceptions;
using DispatchR.Extensions;
using DispatchR.Requests;
using DispatchR.Requests.Stream;
using DispatchR.TestCommon.Fixtures;
using DispatchR.TestCommon.Fixtures.SendRequest;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR.UnitTest;

public class AddDispachRConfigurationTests
{
    [Fact]
    public void Send_ReturnsExpectedResponse_DefaultHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true; 
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = null; // <== this line
            cfg.ExcludeHandlers = null; // <== this line
        });
        
        // Assert
        var countOfAllSimpleHandlers = services
            .Count(p =>
                p.IsKeyedService && 
                p.KeyedImplementationType!.GetInterface(typeof(IStreamRequestHandler<,>).Name, true) is null);
        Assert.True(countOfAllSimpleHandlers > 1);
    }
    
    [Fact]
    public void Send_ReturnsExpectedResponse_IncludeSingleHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true; 
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [Fixture.AnyHandlerRequestWithoutPipeline.GetType()]; // <== this line
        });
        
        // Assert
        var countOfAllSimpleHandlers = services
            .Count(p =>
                p.IsKeyedService && 
                p.KeyedImplementationType!.GetInterface(typeof(IStreamRequestHandler<,>).Name, true) is null);
        Assert.Equal(1, countOfAllSimpleHandlers);
    }
    
    [Fact]
    public void Send_ReturnsExpectedResponse_ExcludeSingleHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true; 
            cfg.RegisterNotifications = false;
            cfg.ExcludeHandlers = [Fixture.AnyHandlerRequestWithoutPipeline.GetType()]; // <== this line
        });
        
        // Assert
        var countOfAllSimpleHandlers = services
            .Count(p =>
                p.IsKeyedService && 
                p.KeyedImplementationType == Fixture.AnyHandlerRequestWithoutPipeline.GetType());
        Assert.Equal(0, countOfAllSimpleHandlers);
    }
    
    [Fact]
    public void Send_ReturnsExpectedResponse_IncludeAndExcludeOneHandlers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [Fixture.AnyHandlerRequestWithoutPipeline.GetType()];
            cfg.ExcludeHandlers = [Fixture.AnyHandlerRequestWithoutPipeline.GetType()];
        });

        // Assert
        var countOfAllSimpleHandlers = services
            .Count(p =>
                p.IsKeyedService && 
                p.KeyedImplementationType == Fixture.AnyHandlerRequestWithoutPipeline.GetType());
        Assert.Equal(0, countOfAllSimpleHandlers);
    }
    
    [Fact]
    public void Send_ThrowsException_WhenIncludeHandlersBeEmpty()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [];
        });

        // Assert
        Assert.Throws<IncludeHandlersCannotBeArrayEmptyException>(act);
    }
    
    [Fact]
    public void Send_ThrowsException_WhenExcludeHandlersBeEmpty()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.ExcludeHandlers = [];
        });

        // Assert
        Assert.Throws<ExcludeHandlersCannotBeArrayEmptyException>(act);
    }
    
    [Fact]
    public async Task Send_UsesPipelineBehaviorsInCorrectOrder_RequestWithMultiplePipelines()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.PipelineOrder =
            [
                typeof(PingValueTaskFirstPipelineBehavior),
                typeof(PingValueTaskSecondPipelineBehavior),
            ];
            cfg.IncludeHandlers = [typeof(PingValueTaskHandler)];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        // Act
        var result = await mediator.Send(new PingValueTask(), CancellationToken.None);
        
        // Assert
        Assert.Equal(1, result);
        Assert.True(PingValueTaskFirstPipelineBehavior.ExecutionTime < PingValueTaskSecondPipelineBehavior.ExecutionTime);
    }
    
    [Fact]
    public void Send_RegisterGenericPipeline_IncludeGenericPipeline()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [Fixture.AnyHandlerRequestWithPipeline.GetType()];
        });

        // Assert
        var countOfAllSimpleHandlers = services
            .Count(p =>
                p.IsKeyedService && 
                p.KeyedImplementationType!.IsGenericType &&
                p.KeyedImplementationType?.GetGenericTypeDefinition() == typeof(GenericPipelineBehavior<,>).GetGenericTypeDefinition());
        Assert.Equal(1, countOfAllSimpleHandlers);
    }
}