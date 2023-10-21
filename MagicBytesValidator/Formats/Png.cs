using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

// TODO: Add trailer check: 49 45 4E 44 AE 42 60 82

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Png : FileTypeWithStartSequences
{
    public Png() : base(
        new[] { "image/png" },
        new[] { "png" },
        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
    )
    {
    }
}