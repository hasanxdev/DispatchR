using DispatchR.Abstractions.Send;
using DispatchR.Abstractions.Stream;
using DispatchR.Extensions;
using DispatchR.TestCommon.Fixtures;
using DispatchR.TestCommon.Fixtures.StreamRequest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace DispatchR.IntegrationTest;

public class StreamRequestHandlerTests
{
    [Fact]
    public async Task CreateStream_UsesPipelineBehavior_WithSinglePipeline()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = true;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [Fixture.AnyStreamHandler.GetType()];
        });
        
        var firstPipeline = services
            .Single(p => p.IsKeyedService && p.KeyedImplementationType == typeof(CounterPipelineStreamHandler));
        
        var spyPipelineOneMock = new Mock<IStreamPipelineBehavior<CounterStreamRequest, string>>();
        spyPipelineOneMock.Setup(p => p.Handle(It.IsAny<CounterStreamRequest>(), It.IsAny<CancellationToken>()))
            .Returns((CounterStreamRequest req, CancellationToken ct) => Fixture.AnyStreamHandler.Handle(req, ct));

        services.RemoveAllKeyed(typeof(IRequestHandler), firstPipeline.ServiceKey);
        
        services.AddKeyedScoped(typeof(IRequestHandler), firstPipeline.ServiceKey!, (_,__) => spyPipelineOneMock.Object);
        
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        // Act
        int counter = 0;
        await foreach (var response in mediator.CreateStream(Fixture.AnyStreamRequest, CancellationToken.None))
        {
            counter++;
        }
        
        // Assert
        Assert.Equal(1, counter);
        spyPipelineOneMock.Verify(p => p.Handle(It.IsAny<CounterStreamRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }
}