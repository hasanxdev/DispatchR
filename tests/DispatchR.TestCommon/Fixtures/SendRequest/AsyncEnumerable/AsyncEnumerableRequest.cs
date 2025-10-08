using DispatchR.Abstractions.Send;

namespace DispatchR.TestCommon.Fixtures.SendRequest.AsyncEnumerable;

public class AsyncEnumerableRequest : IRequest<AsyncEnumerableRequest, IAsyncEnumerable<int>>;
