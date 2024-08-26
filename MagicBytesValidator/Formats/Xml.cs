namespace MagicBytesValidator.Formats;

/// <see href="https://de.wikipedia.org/wiki/Extensible_Markup_Language"/>
public class Xml : FileByteFilter
{
    public Xml() : base(
        ["application/xml", "text/xml"],
        ["xml"]
    )
    {
        StartsWith([0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20]); // "<?xml "
    }
}
