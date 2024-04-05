namespace UrlShortner;

public interface IUrlShorteningService
{
    Task<Code> GenerateUniqueCode();
    Task<string?> GetUrlFromCode(Code code);
}
