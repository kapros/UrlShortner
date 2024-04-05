namespace UrlShortner;

public readonly record struct Code(string code)
{

    public override string ToString()
    {
        return code;
    }

    public static Code Create(char[] codeChars)
    {
        return new Code(new string(codeChars));
    }

    public static Code Empty()
    {
        return new Code(new string(""));
    }

    public static bool TryParse(string code, IFormatProvider formatProvider, out Code result)
    {
        if (string.IsNullOrEmpty(code))
        {
            result = Empty();
            return false;
        }
        result = new Code(code);
        return true;
    }
}
