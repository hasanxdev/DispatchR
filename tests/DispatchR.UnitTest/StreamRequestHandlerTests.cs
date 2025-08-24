using DispatchR.Extensions;
using DispatchR.TestCommon.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR.UnitTest;

public class StreamRequestHandlerTests
{
    [Fact]
    public async Task CreaseStream_StreamHandlerReturnsExpectedResponse_WhenSingleHandlerIsIncluded()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(typeof(Fixture).Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [Fixture.AnyStreamHandler.GetType()];
        });
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
            cfg.IncludeHandlers = [Fixture.AnyStreamHandler.GetType()];
        });
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
    }
}