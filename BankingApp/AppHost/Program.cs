using Projects;

var builder = DistributedApplication.CreateBuilder(args);



var apiService = builder.AddDockerfile("BankingAccountApi", "..", "Banking.AccountApi/Dockerfile");

builder.AddDockerfile("BankingWebUI", "..", "Banking.WebUI/Dockerfile")
    .WithExternalHttpEndpoints()
    .WithReference(apiService.GetEndpoint("Banking.AccountApi"))
    .WaitFor(apiService);

builder.Build().Run();
