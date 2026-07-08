using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed class ConcreteWorkflowNotificationHandler : INotificationHandler<WorkflowNotification<ConcreteWorkflowEvent>>
{
    public ValueTask Handle(WorkflowNotification<ConcreteWorkflowEvent> request, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}