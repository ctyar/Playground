var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Playground>("playground");

builder.Build().Run();
