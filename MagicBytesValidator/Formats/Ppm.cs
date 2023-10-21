using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://en.wikipedia.org/wiki/Portable_bitmap"/>
public class Ppm : FileTypeWithStartSequences
{
    public Ppm() : base(
        new[] { "image/x-portable-pixmap" },
        new[] { "ppm" },
        new[]
        {
            new byte[] { 0x50, 0x33, 0x0A },
            new byte[] { 0x50, 0x36, 0x0A }
        }
    )
    {
    }
}