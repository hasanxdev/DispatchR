using System.Reflection;
using DispatchR;
using Sample;
    
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDispatchR(typeof(MyCommand).Assembly, false);

// builder.Services.AddMediatR(cfg =>
// {
//     cfg.Lifetime = ServiceLifetime.Scoped;
//     cfg.RegisterServicesFromAssemblies(typeof(MyCommand).Assembly);
// });
// builder.Services.AddTransient<MediatR.IPipelineBehavior<MyCommand, int>, PipelineBehavior>();
// builder.Services.AddTransient<MediatR.IPipelineBehavior<MyCommand, int>, Pipeline2>();

var app = builder.Build();
var mediator = app.Services.CreateAsyncScope().ServiceProvider.GetRequiredService<IMediator>();
var tt = await mediator.Send(new MyCommand(), CancellationToken.None);
var mediato2 = app.Services.CreateAsyncScope().ServiceProvider.GetRequiredService<IMediator>();
var tt2 = await mediato2.Send(new MyCommand(), CancellationToken.None);
var tt3 = await mediator.Send(new MyCommand(), CancellationToken.None);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}