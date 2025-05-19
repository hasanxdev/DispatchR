namespace DispatchR.Requests.Stream;

public interface IStreamRequest;

public interface IStreamRequest<TRequest, TResponse> : IStreamRequest where TRequest : class, new();