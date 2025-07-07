# DispatchR Assembly Filtering Example

This example demonstrates how to use the `IncludeHandlers` and `ExcludeHandlers` features of DispatchR to selectively control which handlers are registered from a shared assembly.

This is particularly useful in modular or microservice architectures, where different services might share a common modules library, but each service only needs a specific subset of its functionalities.

## Project Structure

- **`AspireModularSample.Modules`**: A class library containing the `Request`/`Handler` definitions.
  - `Ping` (Request) and `PingHandler`
  - `Pong` (Request) and `PongHandler`
- **`AspireModularSample.ServiceA`**: A service that references `AspireModularSample.Modules` but is configured to **exclude** the `PongHandler`.
- **`AspireModularSample.ServiceB`**: A service that also references `AspireModularSample.Modules` but is configured to **only include** the `PongHandler`.
- **`AspireModularSample.AppHost`**: An Aspire project to orchestrate and run services A and B.

## Configuration

### ServiceA (`ExcludeHandlers`)

`ServiceA` registers all handlers from the `AspireModularSample.Modules` assembly, **except** for the `PongHandler`. This is done using the `ExcludeHandlers` option.

```csharp
// In AspireModularSample.ServiceA/Program.cs

builder.Services.AddDispatchR(options =>
{
    options.Assemblies.Add(Assembly.Load("AspireModularSample.Modules"));
    options.ExcludeHandlers = [typeof(PongHandler)];
});
```

With this setup, `ServiceA` can resolve and execute `Ping`, but an attempt to execute `Pong` would result in an error because its handler was not registered.

### ServiceB (`IncludeHandlers`)

`ServiceB` registers **only** the handlers specified in the `IncludeHandlers` list from the `AspireModularSample.Modules` assembly.

```csharp
// In AspireModularSample.ServiceB/Program.cs

builder.Services.AddDispatchR(options =>
{
    options.Assemblies.Add(Assembly.Load("AspireModularSample.Modules"));
    options.IncludeHandlers = [typeof(PongHandler)];
});
```

With this setup, `ServiceB` can resolve and execute `Pong`, but not `Ping`.

## How to Run

1.  Set `AspireModularSample.AppHost` as the startup project.
2.  Run the project.

Aspire will launch both services. You can then test the endpoints:

- **Service A**: `GET /ping` -> Will return a successful response.
- **Service B**: `GET /pong` -> Will return a successful response.