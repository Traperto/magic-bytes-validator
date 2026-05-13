namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/FLAC"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Flac : FileByteFilter
{
    public Flac() : base(
        ["audio/flac"],
        ["flac"]
    )
    {
        // fLaC magic marker
        StartsWith([0x66, 0x4C, 0x61, 0x43]);
    }
}
