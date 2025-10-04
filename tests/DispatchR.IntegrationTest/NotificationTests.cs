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
}
