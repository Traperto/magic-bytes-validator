using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Bmp : FileTypeWithStartSequences
{
    public Bmp() : base(
        new[] { "image/bmp" },
        new[] { "bmp" },
        new byte[] { 0x42, 0x4D }
    )
    {
    }
}