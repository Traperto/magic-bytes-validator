namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Gif : FileByteFilter
{
    public Gif() : base(
        ["image/gif"],
        ["gif"]
    )
    {
        StartsWithAnyOf([
            [0x47, 0x49, 0x46, 0x38, 0x39, 0x61],
            [0x47, 0x49, 0x46, 0x38, 0x37, 0x61]
        ]);
    }
}