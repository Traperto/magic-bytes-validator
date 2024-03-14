namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Ppt : FileByteFilter
{
    public Ppt() : base(
        ["application/mspowerpoint", "application/vnd.ms-powerpoint"],
        ["ppt", "ppz", "pps", "pot"]
    )
    {
        StartsWith([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
            .SpecificAnyOf([
                new ByteCheck(512, [0xFD, 0xFF, 0xFF, 0xFF, null, null, 0x00, 0x00]),
                new ByteCheck(512, [0xA0, 0x46, 0x1D, 0xF0]),
                new ByteCheck(512, [0x00, 0x6E, 0x1E, 0xF0]),
                new ByteCheck(512, [0x0F, 0x00, 0xE8, 0x03])
            ]);
    }
}