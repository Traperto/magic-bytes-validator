namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://en.wikipedia.org/wiki/Portable_bitmap"/>
public class Ppm : FileByteFilter
{
    public Ppm() : base(
        ["image/x-portable-pixmap"],
        ["ppm"]
    )
    {
        StartsWithAnyOf([
            [0x50, 0x33, 0x0A],
            [0x50, 0x36, 0x0A]
        ]);
    }
}