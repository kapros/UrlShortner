namespace UrlShortner.DataAccess;

public class LinkConverter : ValueConverter<Link, string>
{
    public LinkConverter()
        : base(
            v => v.url,
            v => Link.Create(v))
    {
    }
}
