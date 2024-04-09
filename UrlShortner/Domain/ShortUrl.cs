namespace UrlShortner.Domain;

public class ShortUrl : BaseEntity
{

    public Link Long { get; set; }

    public Link Short { get; set; }

    public Code Code { get; set; }

    public DateTime CreatedOnUtc { get; set; }
}
