using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Ico : FileTypeWithStartSequences
{
    public Ico() : base(
        new[] { "image/x-icon" },
        new[] { "ico" },
        new byte[] { 0x00, 0x00, 0x01, 0x00 }
    )
    {
    }
}