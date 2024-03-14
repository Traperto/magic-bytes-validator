namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Png : FileByteFilter
{
    public Png() : base(
        ["image/png"],
        ["png"]
    )
    {
        StartsWith([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A])
            .EndsWith([0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82]);
    }
}