using DispatchR.Configuration;
using DispatchR.Extensions;
using DispatchR.Requests;
using DispatchR.UnitTest.Fixtures.SendRequest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DispatchR.UnitTest;

public class RequestHandlerTests
{
    [Fact]
    public void Send_ReturnsExpectedResponse_SyncRequestHandler()
    {
    }
    
    [Fact]
    public void Send_ReturnsExpectedResponse_AsyncRequestHandlerWithTask()
    {
    }
    
    [Fact]
    public async Task Send_ReturnsExpectedResponse_AsyncRequestHandlerWithValueTask()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddDispatchR(cfg =>
        {
            cfg.Assemblies.Add(this.GetType().Assembly);
            cfg.RegisterPipelines = false;
            cfg.RegisterNotifications = false;
            cfg.IncludeHandlers = [typeof(PingValueTask)];
        });
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        // Act
        var result = await mediator.Send(new PingValueTask(), CancellationToken.None);
        
        Assert.Equal(1, result);
    }
    
    [Fact]
    public void Send_UsesPipelineBehaviorsInCorrectOrder_RequestWithMultiplePipelines()
    {
    }
    
    [Fact]
    public void Send_ThrowsException_WhenNoHandlerIsRegistered()
    {
    }
    
    [Fact]
    public void Send_UsesCachedHandler_InstanceReusedInScopedLifetime()
    {
    }
}