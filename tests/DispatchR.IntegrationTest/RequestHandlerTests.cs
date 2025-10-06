using DispatchR.Abstractions.Send;
using DispatchR.Extensions;
using DispatchR.TestCommon.Fixtures;
using DispatchR.TestCommon.Fixtures.SendRequest;
using DispatchR.TestCommon.Fixtures.SendRequest.Task;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace DispatchR.IntegrationTest;

public class RequestHandlerTests
{
    [Fact]
    public async Task Send_UsesPipelineBehaviors_RequestWithSinglePipelines()
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
        
        var firstPipeline = services
            .Single(p => p.IsKeyedService && p.KeyedImplementationType == typeof(PingValueTaskFirstPipelineBehavior));
        var secondPipeline = services
            .Single(p => p.IsKeyedService && p.KeyedImplementationType == typeof(PingValueTaskSecondPipelineBehavior));
        
        var spyPipelineTwoMock = new Mock<IPipelineBehavior<PingValueTask, ValueTask<int>>>();
        spyPipelineTwoMock.Setup(p => p.Handle(It.IsAny<PingValueTask>(), It.IsAny<CancellationToken>()))
            .Returns((PingValueTask req, CancellationToken ct) => new PingValueTaskHandler().Handle(req, ct));
        
        var spyPipelineOneMock = new Mock<IPipelineBehavior<PingValueTask, ValueTask<int>>>();
        spyPipelineOneMock.Setup(p => p.Handle(It.IsAny<PingValueTask>(), It.IsAny<CancellationToken>()))
            .Returns((PingValueTask req, CancellationToken ct) => spyPipelineTwoMock.Object.Handle(req, ct));

        services.RemoveAllKeyed(typeof(IRequestHandler), firstPipeline.ServiceKey);
        services.RemoveAllKeyed(typeof(IRequestHandler), secondPipeline.ServiceKey);
        
        services.AddKeyedScoped(typeof(IRequestHandler), firstPipeline.ServiceKey!, (_,__) => spyPipelineTwoMock.Object);
        services.AddKeyedScoped(typeof(IRequestHandler), secondPipeline.ServiceKey!, (_,__) => spyPipelineOneMock.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        
        // Act
        var result = await mediator.Send(new PingValueTask(), CancellationToken.None);
        
        // Assert
        Assert.Equal(1, result);
        spyPipelineOneMock.Verify(p => p.Handle(It.IsAny<PingValueTask>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        spyPipelineTwoMock.Verify(p => p.Handle(It.IsAny<PingValueTask>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public void RegisterHandlers_SingleClassWithMultipleRequestInterfaces_ResolvesAllHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var handlers1 = serviceProvider.GetServices<IRequestHandler<PingTask, Task<int>>>();
        var handlers2 = serviceProvider.GetServices<IRequestHandler<PingValueTask, ValueTask<int>>>();

        // Assert
        Assert.Contains(handlers1, h => h is MultiRequestHandler);
        Assert.Contains(handlers2, h => h is MultiRequestHandler);
    }
}