namespace DispatchR.Requests;

public interface IRequest;

public interface IRequest<TRequest, TResponse> : IRequest where TRequest : class, new();