using UrlShortner.Domain;

namespace UrlShortner.Shorten.Queries;

public class GetShortenedUrlQuery(Code code) : IQuery
{
    public Code Code => code;
}
