using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.Extensions.Options;
using UrlShortner.Shorten.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (builder.Environment.IsDevelopment())
{
    builder.RegisterDevDependencies();
}

builder.AddNonDomainServices();

builder.RegisterUrlServices();

if (!builder.Environment.IsDevelopment())
{
    builder.RegisterStaleUrlsDeletingService();
}
builder.RegisterHandlers();

var corsPolicyName = "localfrontend";
builder.Services.AddCors(options => 
    options.AddPolicy(name: corsPolicyName,
    policy =>
    {
        policy.AllowAnyOrigin();
    }));
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader());
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);
app.UseSerilogRequestLogging();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);


app.MapEndpoints(versionedGroup);

app.Run();


namespace UrlShortner
{
    public partial class Program
    {
        // Expose the Program class for use with WebApplicationFactory<T>
    }
}