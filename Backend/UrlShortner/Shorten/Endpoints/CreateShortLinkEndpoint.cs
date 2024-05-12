namespace UrlShortner.Shorten.Endpoints;

public class CreateShortLinkEndpoint : IEndpoint
{
    public string EndpointName => "create";

    public string EndpointTag => Consts.OPENAPI_TAG;

    public List<int> ApiVersions => [1];

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app) => 
        app.MapPost("shorten", EndpointHandlers.CreateShortLink());
}
