using System.Reflection;
using AspireModularSample.Modules;
using DispatchR;
using DispatchR.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDispatchR(options =>
{
    options.Assemblies.Add(Assembly.Load("AspireModularSample.Modules"));
    options.ExcludeHandlers = [typeof(PongHandler)];
});

var app = builder.Build();

app.MapGet("/ping", (IMediator mediator) => mediator.Send(new Ping(), CancellationToken.None));

app.Run();