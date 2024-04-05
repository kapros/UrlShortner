namespace UrlShortner.DataAccess;

public class ShortUrl
{
    public Guid Id { get; set; }

    public string Long { get; set; } = string.Empty;

    public string Short { get; set; } = string.Empty;

    public Code Code { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}
