using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://en.wikipedia.org/wiki/Portable_bitmap"/>
public class Pbm : FileTypeWithStartSequences
{
    public Pbm() : base(
        new[] { "image/x-portable-bitmap" },
        new[] { "pbm" },
        new[]
        {
            new byte[] { 0x50, 0x31, 0x0A },
            new byte[] { 0x50, 0x34, 0x0A },
        }
    )
    {
    }
}