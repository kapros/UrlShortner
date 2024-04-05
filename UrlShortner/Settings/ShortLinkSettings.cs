namespace UrlShortner.Settings;

public static class ShortUrlSettings
{
    private static int codeLength = 0;

    public static int Length
    {
        get
        {
            if (codeLength == 0)
            {
                int.TryParse(Environment.GetEnvironmentVariable("CodeMaxLength") ?? "7", out codeLength);
            }
            return codeLength;
        }
    }

    public const string Alphabet =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
}
