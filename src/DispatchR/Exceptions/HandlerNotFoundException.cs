namespace DispatchR.Exceptions;

public class HandlerNotFoundException<TRequest, TResponse>() : Exception(
    $"""
    Handler for request of type '{typeof(TRequest).Name}' returning '{typeof(TResponse).Name}' was not found.
    Make sure you have registered a handler that implements IRequestHandler<{typeof(TRequest).Name}, {typeof(TResponse).Name}> in the DI container. 
    """);