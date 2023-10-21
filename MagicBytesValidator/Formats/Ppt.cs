using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

// TODO: Add sub header check (512 byte offset: FD FF FF FF nn nn 00 00 / A0 46 1D F0 / 00 6E 1E F0 / 0F 00 E8 03)

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Ppt : FileTypeWithStartSequences
{
    public Ppt() : base(
        new[] { "application/mspowerpoint", "application/vnd.ms-powerpoint" },
        new[] { "ppt", "ppz", "pps", "pot" },
        new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
    )
    {
    }
}