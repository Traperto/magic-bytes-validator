using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://en.wikipedia.org/wiki/Portable_bitmap"/>
public class Pgm : FileTypeWithStartSequences
{
    public Pgm() : base(
        new[] { "image/x-portable-graymap" },
        new[] { "pgm" },
        new[]
        {
            new byte[] { 0x50, 0x32, 0x0A },
            new byte[] { 0x50, 0x35, 0x0A }
        }
    )
    {
    }
}