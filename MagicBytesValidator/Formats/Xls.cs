using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

// TODO: Add sub header check (512 byte offset: FD FF FF FF nn 00 / FD FF FF FF nn 02 / FD FF FF FF 20 00 00 00)

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Xls : FileTypeWithStartSequences
{
    public Xls() : base(
        new[] { "application/msexcel" },
        new[] { "xls", "xla" },
        new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
    )
    {
    }
}