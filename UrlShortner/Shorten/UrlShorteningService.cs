using UrlShortner.Domain;

namespace UrlShortner.Shorten;

public class UrlShorteningService : IUrlShorteningService
{
    private readonly UrlShortenerDbContext _dbContext;
    private readonly IRandomizer _randomizer;

    public UrlShorteningService(UrlShortenerDbContext dbContext, IRandomizer randomizer)
    {
        _dbContext = dbContext;
        _randomizer = randomizer;
    }

    public async Task<ShortUrl> ShortenUrl(string urlToShorten, HttpRequest httpRequest)
    {
        var code = await GenerateUniqueCode();
        var shortenedUrl = new ShortUrl
        {
            Id = Guid.NewGuid(),
            Long = Link.Create(urlToShorten),
            Code = code,
            Short = Link.Create($"{httpRequest.Scheme}://{httpRequest.Host}/{code}"),
            CreatedOnUtc = DateTime.UtcNow
        };
        _dbContext.ShortenedUrls.Add(shortenedUrl);

        await _dbContext.SaveChangesAsync();
        return shortenedUrl;
    }

    public async Task<Code> GenerateUniqueCode()
    {
        var length = ShortUrlSettings.Length;
        var codeChars = new char[length];
        var maxValue = ShortUrlSettings.Alphabet.Length;

        while (true)
        {
            for (var i = 0; i < length; i++)
            {
                var randomIndex = _randomizer.GetRandom(maxValue);

                codeChars[i] = ShortUrlSettings.Alphabet[randomIndex];
            }
            var code = Code.Create(codeChars);

            if (!await _dbContext.ShortenedUrls.AnyAsync(s => s.Code == code))
            {
                return code;
            }
        }
    }

    public async Task<string?> GetUrlFromCode(Code code)
    {
        var result = await _dbContext
            .ShortenedUrls
            .SingleOrDefaultAsync(s => s.Code == code);
        return result?.Long.ToString();
    }

    public async Task DeleteShortUrl(Code code)
    {
        var entity = await _dbContext.ShortenedUrls.SingleOrDefaultAsync(x => x.Code == code);
        if (entity is not null)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
