namespace DispatchR.Abstractions.Send;

public interface IRequest;

public interface IRequest<TRequest, TResponse> : IRequest where TRequest : class;