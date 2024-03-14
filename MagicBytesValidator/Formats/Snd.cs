namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Snd : FileByteFilter
{
    public Snd() : base(
        ["audio/basic"],
        ["snd", "au"]
    )
    {
        StartsWith([0x2E, 0x73, 0x6E, 0x64]);
    }
}