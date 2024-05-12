namespace UrlShortner.Shorten.Endpoints;

public class GetAllLinksEndpoint : IEndpoint
{
    public string EndpointName => "getAll";

    public string EndpointTag => Consts.OPENAPI_TAG;

    public List<int> ApiVersions => [1];

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("all", EndpointHandlers.GetAll());
}
