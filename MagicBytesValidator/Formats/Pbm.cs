namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://en.wikipedia.org/wiki/Portable_bitmap"/>
public class Pbm : FileByteFilter
{
    public Pbm() : base(
        ["image/x-portable-bitmap"],
        ["pbm"]
    )
    {
        StartsWithAnyOf([
            [0x50, 0x31, 0x0A],
            [0x50, 0x34, 0x0A],
        ]);
    }
}