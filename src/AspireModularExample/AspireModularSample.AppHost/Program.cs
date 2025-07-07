var builder = DistributedApplication.CreateBuilder(args);

var serviceA = builder.AddProject<Projects.AspireModularSample_ServiceA>("ServiceA");
var serviceB = builder.AddProject<Projects.AspireModularSample_ServiceB>("ServiceB");

builder.Build().Run();