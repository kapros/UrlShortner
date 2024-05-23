using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sqlserver")
    .WithDataVolume()
    .AddDatabase("urldb");

var cache = builder.AddRedis("redis")
    .WithRedisCommander()
    .WithDataVolume();

var urlApi = builder.AddProject<Projects.UrlShortner>("shortener");

var frontend = builder.AddNpmApp("frontend", "../../frontend", "dev")
    .WithReference(urlApi)
    .WithEnvironment("BROWSER", "none")
    .WithHttpsEndpoint( env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();

