using DispatchR.Extensions;
using DispatchR.Requests;
using DispatchR.Requests.Notification;
using DispatchR.Requests.Send;
using DispatchR.TestCommon.Fixtures;
using DispatchR.TestCommon.Fixtures.Notification;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
}