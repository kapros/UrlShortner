
namespace UrlShortner.Shorten.Endpoints;

public class DeleteShortLinkEndpoint : IEndpoint
{
    public string EndpointName => "delete";

    public string EndpointTag => Consts.OPENAPI_TAG;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app) => 
        app.MapDelete("{code}", EndpointHandlers.DeleteByCode());
}
