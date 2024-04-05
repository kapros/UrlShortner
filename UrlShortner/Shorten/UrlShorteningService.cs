using UrlShortner.DataAccess;
using UrlShortner.Settings;

namespace UrlShortner.Shorten;

public class UrlShorteningService : IUrlShorteningService
{
    private readonly UrlShortnerDbContext _dbContext;
    private readonly IRandomizer _randomizer;

    public UrlShorteningService(UrlShortnerDbContext dbContext, IRandomizer randomizer)
    {
        _dbContext = dbContext;
        _randomizer = randomizer;
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
        return result?.Long;
    }
}
