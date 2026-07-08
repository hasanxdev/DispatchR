using DispatchR.Abstractions.Notification;

namespace DispatchR.TestCommon.Fixtures.Notification;

public interface IWorkflowNotificationHandler<TNotification> : INotificationHandler<WorkflowNotification<TNotification>>
    where TNotification : IWorkflowEvent;