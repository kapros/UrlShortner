namespace UrlShortner.Domain;

public readonly record struct Link(string url)
{
    public override string ToString()
    {
        return url;
    }

    public static Link Create(char[] codeChars) => new Link(new string(codeChars));

    public static Link Create(string url) => new Link(url);

    public static Link Empty() => new ("");
}
