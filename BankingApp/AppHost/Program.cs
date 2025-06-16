using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Banking_AccountApi>("apiservice");
builder.AddProject<Banking_WebUI>("frontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
