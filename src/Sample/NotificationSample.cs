using System.Threading;
using System.Threading.Tasks;
using DispatchR;
using DispatchR.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Sample;

public class OrderCreatedNotification : INotification
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
}

public class OrderCreatedNotificationHandler : INotificationHandler<OrderCreatedNotification>
{
    public Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken)
    {
        System.Console.WriteLine($"Order {notification.OrderId} created for customer {notification.CustomerName}");
        return Task.CompletedTask;
    }
}

public class NotificationSample
{
    public async Task<bool> Run()
    {
        var services = new ServiceCollection();
        services.AddDispatchR(typeof(NotificationSample).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var notification = new OrderCreatedNotification { OrderId = 1, CustomerName = "John Doe" };
        await mediator.Publish(notification, CancellationToken.None);

        return true;
    }
} 