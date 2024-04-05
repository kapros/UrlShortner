﻿namespace UrlShortner;

public class CodeConverter : ValueConverter<Code, string>
{
    public CodeConverter()
        : base(
            v => v.code,
            v => new Code(v))
    {
    }
}
