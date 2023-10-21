using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Ogv : FileTypeWithStartSequences
{
    public Ogv() : base(
        new[] { "video/ogg" },
        new[] { "ogv", "ogg", "oga" },
        new byte[] { 0x4F, 0x67, 0x67, 0x53, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
    )
    {
    }
}