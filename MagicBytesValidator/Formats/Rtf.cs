using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

// TODO: Add trailer: 7D

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Rtf : FileTypeWithStartSequences
{
    public Rtf() : base(
        new[] { "application/rtf" },
        new[] { "rtf" },
        new byte[] { 0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31 }
    )
    {
    }
}