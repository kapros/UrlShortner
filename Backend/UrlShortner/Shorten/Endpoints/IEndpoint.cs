namespace UrlShortner.Shorten;

public interface IEndpoint
{
    RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app);
    string EndpointName { get; }
    string EndpointTag { get; }
    List<int> ApiVersions { get; }
}