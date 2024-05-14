namespace UrlShortner.DataAccess;

public class CodeConverter : ValueConverter<Code, string>
{
    public CodeConverter()
        : base(
            v => v.code,
            v => Code.Create(v))
    {
    }
}
