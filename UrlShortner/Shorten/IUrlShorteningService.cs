using UrlShortner.DataAccess;

namespace UrlShortner.Shorten;

public interface IUrlShorteningService
{
    Task<Code> GenerateUniqueCode();
    Task<string?> GetUrlFromCode(Code code);
}
