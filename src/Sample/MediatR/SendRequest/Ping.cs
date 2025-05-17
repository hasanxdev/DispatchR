using MediatR;

namespace Sample.MediatR.SendRequest;

public class Ping : IRequest<int>
{
}