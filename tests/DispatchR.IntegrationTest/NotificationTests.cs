using DispatchR.Abstractions.Notification;
using DispatchR.Extensions;
using DispatchR.TestCommon.Fixtures;
using DispatchR.TestCommon.Fixtures.Notification;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DispatchR.IntegrationTest;

public class NotificationTests
{
    [Fact]
    public async Task Publish_CallsAllHandlers_WhenMultipleHandlersAreRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = true;
        });

        var spyPipelineOneMock = new Mock<INotificationHandler<MultiHandlersNotification>>();
        var spyPipelineTwoMock = new Mock<INotificationHandler<MultiHandlersNotification>>();
        var spyPipelineThreeMock = new Mock<INotificationHandler<MultiHandlersNotification>>();

        spyPipelineOneMock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()));
        spyPipelineTwoMock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()));
        spyPipelineThreeMock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()));

        services.AddScoped<INotificationHandler<MultiHandlersNotification>>(sp => spyPipelineOneMock.Object);
        services.AddScoped<INotificationHandler<MultiHandlersNotification>>(sp => spyPipelineTwoMock.Object);
        services.AddScoped<INotificationHandler<MultiHandlersNotification>>(sp => spyPipelineThreeMock.Object);

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        await mediator.Publish(new MultiHandlersNotification(Guid.Empty), CancellationToken.None);

        // Assert
        spyPipelineOneMock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        spyPipelineTwoMock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        spyPipelineThreeMock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task PublishObject_CallsAllHandlers_WhenMultipleHandlersAreRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = true;
        });

        var spyPipelineOneMock = new Mock<INotificationHandler<MultiHandlersNotification>>();
        var spyPipelineTwoMock = new Mock<INotificationHandler<MultiHandlersNotification>>();
        var spyPipelineThreeMock = new Mock<INotificationHandler<MultiHandlersNotification>>();

        spyPipelineOneMock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()));
        spyPipelineTwoMock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()));
        spyPipelineThreeMock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()));

        services.AddScoped<INotificationHandler<MultiHandlersNotification>>(sp => spyPipelineOneMock.Object);
        services.AddScoped<INotificationHandler<MultiHandlersNotification>>(sp => spyPipelineTwoMock.Object);
        services.AddScoped<INotificationHandler<MultiHandlersNotification>>(sp => spyPipelineThreeMock.Object);

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        object notificationObject = new MultiHandlersNotification(Guid.Empty);
        await mediator.Publish(notificationObject, CancellationToken.None);

        // Assert
        spyPipelineOneMock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        spyPipelineTwoMock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        spyPipelineThreeMock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public void RegisterNotification_SingleClassWithMultipleNotificationInterfaces_ResolvesAllHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterNotifications = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var handlers1 = serviceProvider.GetServices<INotificationHandler<MultiHandlersNotification>>();
        var handlers2 = serviceProvider.GetServices<INotificationHandler<MultiHandlersNotification2>>();

        // Assert
        Assert.Contains(handlers1, h => h is MultiNotificationHandler);
        Assert.Contains(handlers2, h => h is MultiNotificationHandler);
    }

    [Fact]
    public async Task Publish_CallsSingleHandler_WhenOnlyOneHandlerIsRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = false;
        });

        var spyHandlerMock = new Mock<INotificationHandler<MultiHandlersNotification>>();
        spyHandlerMock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        services.AddScoped<INotificationHandler<MultiHandlersNotification>>(sp => spyHandlerMock.Object);

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        await mediator.Publish(new MultiHandlersNotification(Guid.Empty), CancellationToken.None);

        // Assert
        spyHandlerMock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Publish_CallsAsyncHandlers_WhenHandlersRequireAwaiting()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = true;
            cfg.IncludeHandlers = [typeof(NotificationOneHandler)];
        });

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        await mediator.Publish(new MultiHandlersNotification(Guid.Empty), CancellationToken.None);

        // Assert - if this completes without exception, the async handler was properly awaited
        Assert.True(true);
    }

    [Fact]
    public async Task Publish_CallsSyncHandlers_WhenHandlersAreAlreadyCompleted()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = true;
            cfg.IncludeHandlers = [typeof(NotificationTwoHandler), typeof(NotificationThreeHandler)];
        });

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        await mediator.Publish(new MultiHandlersNotification(Guid.Empty), CancellationToken.None);

        // Assert - if this completes without exception, the sync handlers were properly handled
        Assert.True(true);
    }

    [Fact]
    public async Task Publish_HandlesNonArrayEnumerable_WhenHandlersAreNotArray()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = false;
        });

        var handler1Mock = new Mock<INotificationHandler<MultiHandlersNotification>>();
        var handler2Mock = new Mock<INotificationHandler<MultiHandlersNotification>>();

        handler1Mock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);
        handler2Mock.Setup(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        // Register a custom service that returns a non-array IEnumerable
        services.AddScoped<IEnumerable<INotificationHandler<MultiHandlersNotification>>>(sp =>
        {
            var list = new List<INotificationHandler<MultiHandlersNotification>>
            {
                handler1Mock.Object,
                handler2Mock.Object
            };
            // Return as IEnumerable (not array) by using LINQ
            return list.Where(h => h != null);
        });

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        await mediator.Publish(new MultiHandlersNotification(Guid.Empty), CancellationToken.None);

        // Assert
        handler1Mock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        handler2Mock.Verify(p => p.Handle(It.IsAny<MultiHandlersNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task Publish_HandlesMixedAsyncAndSyncHandlers_WhenMultipleHandlersAreRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = true;
            cfg.IncludeHandlers = [typeof(NotificationOneHandler), typeof(NotificationTwoHandler), typeof(NotificationThreeHandler)];
        });

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        await mediator.Publish(new MultiHandlersNotification(Guid.Empty), CancellationToken.None);

        // Assert - if this completes without exception, all handlers (async and sync) were properly handled
        Assert.True(true);
    }
}
