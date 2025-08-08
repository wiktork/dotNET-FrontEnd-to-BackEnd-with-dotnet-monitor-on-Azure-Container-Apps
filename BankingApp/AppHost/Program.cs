using Aspire.Hosting;
using Projects;


// Additional steps required for successfull build:
// dotnet user-secret set pat <PAT that has package read from devdiv>
// modified procdump build

var builder = DistributedApplication.CreateBuilder(args);
var parameter = builder.AddParameter("pat", value:builder.Configuration["pat"]);

var apiService = builder.AddDockerfile("bankservice", "..", "Banking.AccountApi/Dockerfile", "final");
apiService.WithEndpoint(targetPort: 8080, scheme: "http");

var uiBuilder = builder.AddDockerfile("bankui", "..", "Banking.WebUI/Dockerfile", "final");
    uiBuilder.WithEndpoint(targetPort: 8080, scheme: "http", isProxied: true, isExternal: true)
    .WithBuildArg("FEED_PAT", parameter)
    .WithReference(apiService.GetEndpoint("bankservice"))
    .WaitFor(apiService);


builder.Build().Run();
