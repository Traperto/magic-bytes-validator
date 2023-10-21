using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Exe : FileTypeWithStartSequences
{
    public Exe() : base(
        new[] { "application/x-dosexec", "application/x-msdos-program" },
        new[]
        {
            "exe", "com", "dll", "drv", "pif", "qts", "qtx ", "sys", "acm", "ax", "cpl", "fon", "ocx", "olb", "scr",
            "vbx", "vxd", "mui", "iec", "ime", "rs", "tsp", "efi"
        },
        new byte[] { 0x4D, 0x5A }
    )
    {
    }
}