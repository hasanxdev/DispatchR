using System.Reflection;
using AspireModularSample.Modules;
using DispatchR.Extensions;
using DispatchR.Requests;
using DispatchR.Requests.Send;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDispatchR(options =>
{
    options.Assemblies.Add(Assembly.Load("AspireModularSample.Modules"));
    options.ExcludeHandlers = [typeof(PongHandler)];
});

var app = builder.Build();

app.MapGet("/ping", (IMediator mediator) => mediator.Send(new Ping(), CancellationToken.None));

app.Run();