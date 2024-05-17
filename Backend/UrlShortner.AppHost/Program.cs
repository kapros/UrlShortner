using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var urlApi = builder.AddProject<Projects.UrlShortner>("shortener");

builder.AddNpmApp("react", "../../frontend", "dev")
    .WithReference(urlApi)
    .WithEnvironment("BROWSER", "none")
    .WithHttpsEndpoint( env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
