using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public sealed record WorkflowNotification<TNotification>(TNotification Event) : INotification
    where TNotification : IWorkflowEvent;