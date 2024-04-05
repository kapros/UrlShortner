namespace UrlShortner.Shorten;

public class Randomizer : IRandomizer
{
    private readonly Random _random;
    public Randomizer() => _random = new Random(DateTime.Now.Minute * DateTime.Now.Hour);
    public int GetRandom(int maxValue) => _random.Next(maxValue);
}
