using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

// TODO: Add trailer checks (0A 25 25 45 4F 46 / 0A 25 25 45 4F 46 0A / 0D 0A 25 25 45 4F 46 0D 0A / 0D 25 25 45 4F 46 0D)

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Pdf : FileTypeWithStartSequences
{
    public Pdf() : base(
        new[] { "application/pdf" },
        new[] { "pdf" },
        new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }
    )
    {
    }
}