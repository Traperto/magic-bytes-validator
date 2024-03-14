namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Mpg : FileByteFilter
{
    public Mpg() : base(
        new[] { "video/mpeg" },
        new[] { "mpg", "mpeg", "mpe", "m2p", "vob" }
    )
    {
        StartsWithAnyOf([
            [0x00, 0x00, 0x01, 0xB3],
            [0x00, 0x00, 0x01, 0xBA]
        ]);
    }
}