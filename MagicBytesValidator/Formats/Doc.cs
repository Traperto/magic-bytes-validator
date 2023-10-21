using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

// TODO: Add sub header check (512 byte offset: EC A5 C1 00)

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Doc : FileTypeWithStartSequences
{
    public Doc() : base(
        new[] { "application/msword" },
        new[] { "doc", "dot" },
        new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
    )
    {
    }
}