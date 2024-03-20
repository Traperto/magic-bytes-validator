namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Bmp : FileByteFilter
{
    public Bmp() : base(
        ["image/bmp"],
        ["bmp"]
    )
    {
        StartsWith([0x42, 0x4D]);
    }
}