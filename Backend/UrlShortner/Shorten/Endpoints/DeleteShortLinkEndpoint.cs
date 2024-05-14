namespace UrlShortner.Shorten.Endpoints;

public class DeleteShortLinkEndpoint : IEndpoint
{
    public string EndpointName => "delete";

    public string EndpointTag => Consts.OPENAPI_TAG;

    public List<int> ApiVersions => [1];

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app) => 
        app.MapDelete("{code}", EndpointHandlers.DeleteByCode());
}
