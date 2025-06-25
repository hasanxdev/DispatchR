using DispatchR.Extensions;
using Scalar.AspNetCore;
using DispatchRNotificationSample = Sample.DispatchR.Notification;
using DispatchRSample = Sample.DispatchR.SendRequest;
using DispatchRStreamSample = Sample.DispatchR.StreamRequest;
using MediatRNotificationSample = Sample.MediatR.Notification;
using MediatRSample = Sample.MediatR.SendRequest;
using MediatRStreamSample = Sample.MediatR.StreamRequest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMediatR(cfg =>
{
    cfg.Lifetime = ServiceLifetime.Scoped;
    cfg.RegisterServicesFromAssemblies(typeof(MediatRSample.Ping).Assembly);
});
builder.Services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(MediatRSample.GenericPipelineBehavior<,>));
builder.Services.AddTransient<MediatR.IPipelineBehavior<MediatRSample.Ping, int>, MediatRSample.FirstPipelineBehavior>();
builder.Services.AddTransient<MediatR.IPipelineBehavior<MediatRSample.Ping, int>, MediatRSample.SecondPipelineBehavior>();
builder.Services.AddTransient<MediatR.IStreamPipelineBehavior<MediatRStreamSample.CounterStreamRequest, string>, MediatRStreamSample.CounterPipelineStreamHandler>();

builder.Services.AddDispatchR(options =>
{
    options.Assemblies.Add(typeof(DispatchRSample.Ping).Assembly);
    options.RegisterPipelines = true;
    options.RegisterNotifications = true;
    options.PipelineOrder =
    [
        typeof(DispatchRSample.FirstPipelineBehavior),
        typeof(DispatchRSample.SecondPipelineBehavior),
        typeof(DispatchRSample.GenericPipelineBehavior<,>)
    ];
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.MapGet("/Send/MediatR", (MediatR.IMediator mediatR, CancellationToken cancellationToken)
        => mediatR.Send(new MediatRSample.Ping(), cancellationToken))
    .WithName("SendInMediatRWithPipeline");

app.MapGet("/Send/DispatchR", (DispatchR.Requests.IMediator dispatchR, CancellationToken cancellationToken)
        => dispatchR.Send(new DispatchRSample.Ping(), cancellationToken))
    .WithName("SendInDispatchRWithPipeline");

app.MapGet("/Stream/MediatR", async (MediatR.IMediator mediatR, ILogger<Program> logger) =>
{
    CancellationTokenSource cts = new();
    int count = 0;
    await foreach (var item in mediatR.CreateStream(new MediatRStreamSample.CounterStreamRequest(), cts.Token))
    {
        count++;
        if (item.Contains("CodeWithHSN"))
        {
            cts.Cancel();
        }

        logger.LogInformation($"stream count in MediatR: {count}");
    }

    return "It works";
});

app.MapGet("/Stream/DispatchR", async (DispatchR.Requests.IMediator dispatchR, ILogger<Program> logger) =>
{
    CancellationTokenSource cts = new();
    int count = 0;
    await foreach (var item in dispatchR.CreateStream(new DispatchRStreamSample.CounterStreamRequest(), cts.Token))
    {
        count++;
        if (item.Contains("CodeWithHSN"))
        {
            cts.Cancel();
        }

        logger.LogInformation($"stream count in DispatchR: {count}");
    }

    return "It works";
});

app.MapGet("/Notification/MediatR", async (MediatR.IMediator mediator, ILogger<Program> logger) =>
{
    await mediator.Publish(new MediatRNotificationSample.MultiHandlersNotification(Guid.Empty));
    return "It works";
});

app.MapGet("/Notification/DispatchR", async (DispatchR.Requests.IMediator mediator, ILogger<Program> logger) =>
{
    await mediator.Publish(new DispatchRNotificationSample.MultiHandlersNotification(Guid.Empty), CancellationToken.None);
    return "It works";
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}