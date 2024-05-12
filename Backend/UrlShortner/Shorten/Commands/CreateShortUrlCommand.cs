namespace UrlShortner.Shorten.Commands;

public class CreateShortUrlCommand(string url, HttpRequest httpRequest) : ICommand
{
    public string Url => url;
    public HttpRequest HttpRequest => httpRequest;
}
