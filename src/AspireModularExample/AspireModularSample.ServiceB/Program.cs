using System.Reflection;
using AspireModularSample.Modules;
using DispatchR.Extensions;
using DispatchR.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDispatchR(options =>
{
    options.Assemblies.Add(Assembly.Load("AspireModularSample.Modules"));
    options.IncludeHandlers = [typeof(PongHandler)];
});

var app = builder.Build();

app.MapGet("/pong", (IMediator mediator) => mediator.Send(new Pong(), CancellationToken.None));

app.Run();