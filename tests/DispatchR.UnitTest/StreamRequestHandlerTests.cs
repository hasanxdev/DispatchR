using DispatchR.Configuration;
using DispatchR.Exceptions;
using DispatchR.Extensions;
using DispatchR.Requests;
using DispatchR.TestCommon.Fixtures;
using DispatchR.TestCommon.Fixtures.SendRequest;
using DispatchR.TestCommon.Fixtures.SendRequest.ReusedInScopedLifetime;
using DispatchR.TestCommon.Fixtures.SendRequest.Sync;
using DispatchR.TestCommon.Fixtures.SendRequest.Task;
using DispatchR.TestCommon.Fixtures.SendRequest.ValueTask;
using Microsoft.Extensions.DependencyInjection;
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