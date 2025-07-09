using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddDockerfile("bankservice", "..", "Banking.AccountApi/Dockerfile", "final");
apiService.WithEndpoint(targetPort: 8080, scheme: "http");

var uiBuilder = builder.AddDockerfile("bankui", "..", "Banking.WebUI/Dockerfile", "final");
    uiBuilder.WithEndpoint(targetPort: 8080, scheme: "http", isProxied: true, isExternal: true)
    .WithReference(apiService.GetEndpoint("bankservice"))
    .WaitFor(apiService);


//.WithBuildSecret("FEED_PAT", parameter)

builder.Build().Run();
