
namespace UrlShortner.Shorten.Endpoints;

public class GetShortLinkEndpoint : IEndpoint
{
    public string EndpointName => "get";

    public string EndpointTag => Consts.OPENAPI_TAG;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("{code}", EndpointHandlers.GetByCode());
}
