using DispatchR.Exceptions;
using DispatchR.Extensions;
using DispatchR.TestCommon.Fixtures;
using DispatchR.TestCommon.Fixtures.SendRequest;
using DispatchR.TestCommon.Fixtures.SendRequest.ReusedInScopedLifetime;
using DispatchR.TestCommon.Fixtures.SendRequest.Sync;
using DispatchR.TestCommon.Fixtures.SendRequest.Task;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR.UnitTest;

public class RequestHandlerTests
{
    [Fact]
    public void Send_ReturnsExpectedResponse_SyncRequestHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(PingHandler)];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var result = mediator.Send(new Ping(), CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Send_ReturnsExpectedResponse_AsyncRequestHandlerWithTask()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(PingTaskHandler)];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new PingTask(), CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Send_ReturnsExpectedResponse_AsyncRequestHandlerWithValueTask()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(PingValueTaskHandler)];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new PingValueTask(), CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Send_UsesPipelineBehaviors_RequestWithPipelines()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(PingValueTaskHandler)];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new PingValueTask(), CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Send_UsesPipelineBehaviors_RequestWithOutResponseWithPipelines()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [Fixture.AnyHandlerRequestWithoutResponseWithPipeline.GetType()];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        await mediator.Send(Fixture.AnyRequestWithoutResponsePipeline, CancellationToken.None);

        // Assert
        // Just checking if it runs without exceptions
    }

    [Fact]
    public async Task Send_UsesPipelineBehaviors_ChangePipelineOrdering()
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
                typeof(PingValueTaskFirstPipelineBehavior)
            ];
            cfg.IncludeHandlers = [Fixture.AnyHandlerRequestWithPipeline.GetType()];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new PingValueTask(), CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Send_ThrowsException_WhenNoHandlerIsRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(RequestWithoutHandler)];
        });

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        void Action() => mediator.Send(new RequestWithoutHandler(), CancellationToken.None);

        // Assert
        var exception = Assert.Throws<HandlerNotFoundException<RequestWithoutHandler, int>>(Action);
        Assert.Equal("""
                     Handler for request of type 'RequestWithoutHandler' returning 'Int32' was not found.
                     Make sure you have registered a handler that implements IRequestHandler<RequestWithoutHandler, Int32> in the DI container. 
                     """, exception.Message);
    }

    [Fact]
    public void Send_UsesCachedHandler_InstanceReusedInScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(RequestReusedInScopedLifetimeHandler)];
        });
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Act
        var first = mediator.Send(new RequestReusedInScopedLifetime(), CancellationToken.None);
        var second = mediator.Send(new RequestReusedInScopedLifetime(), CancellationToken.None);
        var third = mediator.Send(new RequestReusedInScopedLifetime(), CancellationToken.None);

        // Assert
        Assert.Equal(3, first + second + third);
    }

    [Fact]
    public void Send_ReturnsSingleHandler_WhenNoPipelinesAreRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(PingHandler)];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var result = mediator.Send(new Ping(), CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Send_ReturnsSingleHandler_WhenOnlyOneHandlerExistsWithPipelinesEnabled()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(PingTaskHandler)]; // Handler without pipeline behaviors
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        var result = await mediator.Send(new PingTask(), CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }
}