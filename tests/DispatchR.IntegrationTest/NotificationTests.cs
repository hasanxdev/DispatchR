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
    public async Task Publish_CallsOpenGenericAndSpecificHandlers_WhenBothAreRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<OpenGenericNotificationExecutionStore>();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executionStore = serviceProvider.GetRequiredService<OpenGenericNotificationExecutionStore>();

        // Act
        await mediator.Publish(new OpenGenericTargetNotification(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Equal(1, executionStore.Count($"generic:{nameof(OpenGenericTargetNotification)}"));
        Assert.Equal(1, executionStore.Count($"specific:{nameof(OpenGenericTargetNotification)}"));
    }

    [Fact]
    public async Task PublishObject_CallsOpenGenericAndSpecificHandlers_WhenBothAreRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<OpenGenericNotificationExecutionStore>();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executionStore = serviceProvider.GetRequiredService<OpenGenericNotificationExecutionStore>();

        // Act
        object notificationObject = new OpenGenericTargetNotification(Guid.NewGuid());
        await mediator.Publish(notificationObject, CancellationToken.None);

        // Assert
        Assert.Equal(1, executionStore.Count($"generic:{nameof(OpenGenericTargetNotification)}"));
        Assert.Equal(1, executionStore.Count($"specific:{nameof(OpenGenericTargetNotification)}"));
    }

    [Fact]
    public async Task Publish_CallsOpenGenericHandler_WhenNoSpecificHandlerExists()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<OpenGenericNotificationExecutionStore>();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var executionStore = serviceProvider.GetRequiredService<OpenGenericNotificationExecutionStore>();

        // Act
        await mediator.Publish(new OpenGenericOnlyNotification(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.Equal(1, executionStore.Count($"generic:{nameof(OpenGenericOnlyNotification)}"));
        Assert.Equal(0, executionStore.Count($"specific:{nameof(OpenGenericOnlyNotification)}"));
    }
}
